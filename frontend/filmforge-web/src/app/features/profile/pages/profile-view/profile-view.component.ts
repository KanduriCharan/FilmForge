import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { ProfileResponse, ProfileService } from '../../../../core/services/profile.service';
import { PostResponse, PostService } from '../../../../core/services/post.service';

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
  private postService = inject(PostService);

  profile: ProfileResponse | null = null;
  userPosts: PostResponse[] = [];
  errorMessage = '';
  isLoadingPosts = false;
  currentUserId = localStorage.getItem('filmforge_userId') ?? '';

  get isOwnProfile(): boolean {
    return !!this.profile && this.profile.userId === this.currentUserId;
  }

  get postCount(): number {
    return this.userPosts.length;
  }

  ngOnInit(): void {
    const username = this.route.snapshot.paramMap.get('username');

    if (!username) {
      this.errorMessage = 'Username not provided.';
      return;
    }

    this.profileService.getProfileByUsername(username).subscribe({
      next: (response) => {
        this.profile = response;
        this.loadUserPosts(response.userId);
      },
      error: (error) => {
        console.log('Profile load error:', error);
        this.errorMessage =
          error?.error?.Message ||
          `Failed to load profile. Status: ${error?.status}`;
      }
    });
  }

  private loadUserPosts(userId: string): void {
    this.isLoadingPosts = true;

    this.postService.getPostsByUserId(userId).subscribe({
      next: (posts) => {
        this.userPosts = posts;
        this.isLoadingPosts = false;
      },
      error: (error) => {
        console.log('User posts load error:', error);
        this.isLoadingPosts = false;
      }
    });
  }

  getPrimaryMediaUrl(post: PostResponse): string {
    return post.mediaItems?.[0]?.mediaUrl || '';
  }

  getPrimaryMediaType(post: PostResponse): string {
    return post.mediaItems?.[0]?.mediaType || '';
  }

  getProfileInitial(): string {
    return (
      this.profile?.fullName?.charAt(0)?.toUpperCase() ||
      this.profile?.username?.charAt(0)?.toUpperCase() ||
      'F'
    );
  }
}