import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';
import { ProfileService } from '../../../../core/services/profile.service'


@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private profileService = inject(ProfileService);

  errorMessage = '';
  successMessage = '';

  loginForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]]
  });
  router: any;

  onSubmit(): void {
    console.log('onSubmit fired');
    this.errorMessage = '';
    this.successMessage = '';
  
    if (this.loginForm.invalid) {
      console.log('Form is invalid', this.loginForm.value);
      this.loginForm.markAllAsTouched();
      return;
    }
  
    const payload = {
      email: this.loginForm.value.email ?? '',
      password: this.loginForm.value.password ?? ''
    };
  
    console.log('Payload being sent:', payload);
  
    this.authService.login(payload).subscribe({
      next: (response) => {
        console.log('Login success:', response);
        localStorage.setItem('filmforge_userId', response.userId);
        localStorage.setItem('filmforge_username', response.username);
        localStorage.setItem('filmforge_email', response.email);

        this.profileService.getProfileByUserId(response.userId).subscribe({
          next: (profile) => {
            this.router.navigate(['/profile', profile.username]);
          },
          error: (error) => {
            console.log('Profile not found, redirecting to onboarding', error);
            this.router.navigate(['/onboarding']);
          }
        });
      },
      error: (error) => {
        console.log('Login failed:', error);
        this.errorMessage = error?.error?.Message || 'Login failed.';
      }
    });
  }
  
}