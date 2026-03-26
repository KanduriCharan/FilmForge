import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators,AbstractControl, ValidationErrors } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.css'
})
export class SignupComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  errorMessage = '';
  successMessage = '';

  signupForm = this.fb.group({
    fullName: ['', [Validators.required]],
    username: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['',Validators.required]
  },
  { validators: this.passwordMatchValidator }
);

  passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.get('password')?.value;
    const confirmPassword = control.get('confirmPassword')?.value;

    if (password !== confirmPassword) {
      return { passwordMismatch: true };
    }

    return null;
  }

  onSubmit(): void {
    this.errorMessage = '';
    this.successMessage = '';
  
    if (this.signupForm.invalid) {
      this.signupForm.markAllAsTouched();
      return;
    }
  
    const payload = {
      fullName: this.signupForm.value.fullName ?? '',
      username: this.signupForm.value.username ?? '',
      email: this.signupForm.value.email ?? '',
      password: this.signupForm.value.password ?? ''
    };
  
    this.authService.signup(payload).subscribe({
      next: (response) => {
        console.log('Signup success:', response);
        localStorage.setItem('filmforge_userId', response.userId);
        localStorage.setItem('filmforge_username', response.username);
        localStorage.setItem('filmforge_email', response.email);
        this.successMessage = response.message;
        this.router.navigate(['/onboarding']);
      },
      error: (error) => {
        console.log('Signup error:', error);
        this.errorMessage = error?.error?.Message || 'Signup failed.';
      }
    });
  }
}