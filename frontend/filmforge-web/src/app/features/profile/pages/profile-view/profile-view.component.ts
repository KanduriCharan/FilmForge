import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { ProfileResponse, ProfileService } from '../../../../core/services/profile.service';

@Component({
  selector: 'app-profile-view',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './profile-view.component.html',
  styleUrl: './profile-view.component.css'
})
export class ProfileViewComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private profileService = inject(ProfileService);

  profile: ProfileResponse | null = null;
  errorMessage = '';

  ngOnInit(): void {
    const username = this.route.snapshot.paramMap.get('username');

    if (!username) {
      this.errorMessage = 'Username not provided.';
      return;
    }

    this.profileService.getProfileByUsername(username).subscribe({
      next: (response) => {
        this.profile = response;
      },
      error: (error) => {
        console.log('Profile load error:', error);
        this.errorMessage =
          error?.error?.Message ||
          `Failed to load profile. Status: ${error?.status}`;
      }
    });
  }
}