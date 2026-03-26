import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ProfileService } from '../../../../core/services/profile.service';

@Component({
  selector: 'app-onboarding',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './onboarding.component.html',
  styleUrl: './onboarding.component.css'
})
export class OnboardingComponent {
  private fb = inject(FormBuilder);
  private profileService = inject(ProfileService);
  private router = inject(Router);

  errorMessage = '';
  successMessage = '';

  onboardingForm = this.fb.group({
    fullName: ['', [Validators.required]],
    username: ['', [Validators.required]],
    bio: [''],
    primaryCraft: ['', [Validators.required]],
    secondaryCrafts: [''],
    location: [''],
    experienceLevel: [''],
    availabilityStatus: [''],
    portfolioUrl: [''],
    instagramUrl: [''],
    youtubeUrl: ['']
  });

  onSubmit(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (this.onboardingForm.invalid) {
      this.onboardingForm.markAllAsTouched();
      return;
    }

    const payload = {
      userId: localStorage.getItem('filmforge_userId') ??'',
      fullName: this.onboardingForm.value.fullName ?? '',
      username: this.onboardingForm.value.username ?? '',
      bio: this.onboardingForm.value.bio ?? '',
      primaryCraft: this.onboardingForm.value.primaryCraft ?? '',
      secondaryCrafts: this.onboardingForm.value.secondaryCrafts ?? '',
      location: this.onboardingForm.value.location ?? '',
      experienceLevel: this.onboardingForm.value.experienceLevel ?? '',
      availabilityStatus: this.onboardingForm.value.availabilityStatus ?? '',
      portfolioUrl: this.onboardingForm.value.portfolioUrl ?? '',
      instagramUrl: this.onboardingForm.value.instagramUrl ?? '',
      youtubeUrl: this.onboardingForm.value.youtubeUrl ?? ''
    };

    this.profileService.createProfile(payload).subscribe({
      next: (response) => {
        this.successMessage = 'Profile created successfully.';
        this.router.navigate(['/studio']);
      },
      error: (error) => {
        this.errorMessage = error?.error?.Message || 'Failed to create profile.';
      }
    });
  }
}