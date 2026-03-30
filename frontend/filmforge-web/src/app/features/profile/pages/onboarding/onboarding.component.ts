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
  isSubmitting = false;

  selectedProfileImageFile: File | null = null;
  profileImagePreviewUrl: string | null = null;

  onboardingForm = this.fb.group({
    fullName: ['', [Validators.required]],
    username: [
      { value: localStorage.getItem('filmforge_username') ?? '', disabled: true },
      [Validators.required]
    ],
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

  onProfileImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;

    if (!file) {
      return;
    }

    this.selectedProfileImageFile = file;

    const reader = new FileReader();
    reader.onload = () => {
      this.profileImagePreviewUrl = reader.result as string;
    };
    reader.readAsDataURL(file);
  }

  removeSelectedImage(): void {
    this.selectedProfileImageFile = null;
    this.profileImagePreviewUrl = null;
  }

  onSubmit(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (this.onboardingForm.invalid) {
      this.onboardingForm.markAllAsTouched();
      return;
    }

    const userId = localStorage.getItem('filmforge_userId') ?? '';

    if (!userId) {
      this.errorMessage = 'User session not found. Please log in again.';
      return;
    }

    this.isSubmitting = true;

    const raw = this.onboardingForm.getRawValue();

    const payload = {
      userId,
      fullName: raw.fullName ?? '',
      username: raw.username ?? '',
      bio: raw.bio ?? '',
      primaryCraft: raw.primaryCraft ?? '',
      secondaryCrafts: raw.secondaryCrafts ?? '',
      location: raw.location ?? '',
      experienceLevel: raw.experienceLevel ?? '',
      availabilityStatus: raw.availabilityStatus ?? '',
      portfolioUrl: raw.portfolioUrl ?? '',
      instagramUrl: raw.instagramUrl ?? '',
      youtubeUrl: raw.youtubeUrl ?? ''
    };

    this.profileService.createProfile(payload).subscribe({
      next: () => {
        const imageFile = this.selectedProfileImageFile;

        if (!imageFile) {
          this.successMessage = 'Profile created successfully.';
          this.isSubmitting = false;
          this.router.navigate(['/studio']);
          return;
        }

        this.profileService.uploadProfileImage(userId, imageFile).subscribe({
          next: () => {
            this.successMessage = 'Profile created successfully.';
            this.isSubmitting = false;
            this.router.navigate(['/studio']);
          },
          error: (error) => {
            console.log('Profile image upload failed:', error);
            this.isSubmitting = false;
            this.errorMessage =
              error?.error?.Message ||
              'Profile created, but image upload failed.';
          }
        });
      },
      error: (error) => {
        this.isSubmitting = false;
        this.errorMessage = error?.error?.Message || 'Failed to create profile.';
      }
    });
  }
}