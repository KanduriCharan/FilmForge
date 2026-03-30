import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Router } from '@angular/router';
import {
  PostCommentResponse,
  PostResponse,
  PostService
} from '../../../core/services/post.service';
import { ProfileResponse, ProfileService } from '../../../core/services/profile.service';
import { FormsModule } from '@angular/forms';

interface OnlineUser {
  name: string;
  craft: string;
}

interface CommentViewModel extends PostCommentResponse {
  authorName: string;
  username: string;
}

interface PostViewModel extends PostResponse {
  authorName: string;
  username: string;
  craft: string;
  commentsOpen: boolean;
  commentsLoading: boolean;
  commentsLoaded: boolean;
  commentsError: string;
  newComment: string;
  comments: CommentViewModel[];
}

@Component({
  selector: 'app-studio',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './studio.component.html',
  styleUrl: './studio.component.css'
})
export class StudioComponent implements OnInit {
  private postService = inject(PostService);
  private profileService = inject(ProfileService);
  private router = inject(Router);

  currentUsername = localStorage.getItem('filmforge_username') ?? '';
  currentUserId = localStorage.getItem('filmforge_userId') ?? '';
  isChatCollapsed = false;

  caption = '';
  selectedFiles: File[] = [];
  selectedFileNames: string[] = [];
  previewUrls: string[] = [];
  isPreviewOpen = false;

  isCreatingPost = false;
  postErrorMessage = '';
  posts: PostViewModel[] = [];
  isLoadingPosts = false;

  onlineUsers: OnlineUser[] = [
    { name: 'Maya Reddy', craft: 'Screenwriting' },
    { name: 'Aarav Menon', craft: 'Cinematography' },
    { name: 'Sofia Blake', craft: 'Editing' },
    { name: 'Jordan Lee', craft: 'Sound Design' },
    { name: 'Ritika Sharma', craft: 'Directing' }
  ];

  ngOnInit(): void {
    this.loadPosts();
  }

  toggleChat(): void {
    this.isChatCollapsed = !this.isChatCollapsed;
  }

  onFilesSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const files = Array.from(input.files ?? []);

    this.clearPreviewUrls();

    if (!files.length) {
      this.selectedFiles = [];
      this.selectedFileNames = [];
      return;
    }

    const imageFiles = files.filter((file) => file.type.startsWith('image/'));
    const videoFiles = files.filter((file) => file.type.startsWith('video/'));

    if (imageFiles.length > 0 && videoFiles.length > 0) {
      this.postErrorMessage = 'You can upload either images or one video, not both.';
      this.selectedFiles = [];
      this.selectedFileNames = [];
      input.value = '';
      return;
    }

    if (videoFiles.length > 1) {
      this.postErrorMessage = 'Only one video is allowed per post.';
      this.selectedFiles = [];
      this.selectedFileNames = [];
      input.value = '';
      return;
    }

    if (files.length > 5) {
      this.postErrorMessage = 'You can upload up to 5 files only.';
      this.selectedFiles = [];
      this.selectedFileNames = [];
      input.value = '';
      return;
    }

    this.postErrorMessage = '';
    this.selectedFiles = files;
    this.selectedFileNames = files.map((file) => file.name);
    this.previewUrls = files.map((file) => URL.createObjectURL(file));
  }

  openPreview(): void {
    this.postErrorMessage = '';

    const trimmedCaption = this.caption.trim();

    if (!trimmedCaption && this.selectedFiles.length === 0) {
      this.postErrorMessage = 'Write something or attach media before posting.';
      return;
    }

    this.isPreviewOpen = true;
  }

  goToMyProfile(): void {
    const userId = localStorage.getItem('filmforge_userId') ?? '';
  
    if (!userId) {
      console.error('No logged-in userId found.');
      return;
    }
  
    this.profileService.getProfileByUserId(userId).subscribe({
      next: (profile) => {
        this.router.navigate(['/profile', profile.username]);
      },
      error: (error) => {
        console.error('Failed to resolve my profile:', error);
      }
    });
  }

  closePreview(): void {
    this.isPreviewOpen = false;
  }

  createPost(): void {
    this.postErrorMessage = '';

    if (!this.currentUserId) {
      this.postErrorMessage = 'User not found. Please log in again.';
      return;
    }

    const trimmedCaption = this.caption.trim();

    if (!trimmedCaption && this.selectedFiles.length === 0) {
      this.postErrorMessage = 'Write something or attach media before posting.';
      return;
    }

    this.isCreatingPost = true;

    this.postService.createPost(this.currentUserId, trimmedCaption, this.selectedFiles).subscribe({
      next: () => {
        this.caption = '';
        this.selectedFiles = [];
        this.selectedFileNames = [];
        this.clearPreviewUrls();
        this.isPreviewOpen = false;
        this.isCreatingPost = false;
        this.loadPosts();
      },
      error: (error) => {
        this.isCreatingPost = false;
        this.postErrorMessage = error?.error?.Message || 'Failed to create post.';
      }
    });
  }

  loadPosts(): void {
    this.isLoadingPosts = true;

    this.postService.getPosts().subscribe({
      next: (posts) => {
        const uniqueUserIds = [...new Set(posts.map((post) => post.userId))];

        if (uniqueUserIds.length === 0) {
          this.posts = [];
          this.isLoadingPosts = false;
          return;
        }

        const profileRequests = uniqueUserIds.map((userId) =>
          this.profileService.getProfileByUserId(userId).toPromise().then(
            (profile) => ({ userId, profile }),
            () => ({ userId, profile: null as ProfileResponse | null })
          )
        );

        Promise.all(profileRequests).then((results) => {
          const profileMap = new Map<string, ProfileResponse | null>();

          results.forEach((result) => {
            profileMap.set(result.userId, result.profile ?? null);
          });

          this.posts = posts.map((post) => {
            const profile = profileMap.get(post.userId) ?? null;

            return {
              ...post,
              authorName: profile?.fullName ?? 'Unknown Creator',
              username: profile?.username ?? 'unknown',
              craft: profile?.primaryCraft ?? 'Creator',
              commentsOpen: false,
              commentsLoading: false,
              commentsLoaded: false,
              commentsError: '',
              newComment: '',
              comments: []
            };
          });

          this.isLoadingPosts = false;
        });
      },
      error: () => {
        this.posts = [];
        this.isLoadingPosts = false;
      }
    });
  }

  likePost(post: PostViewModel): void {
    if (!this.currentUserId) return;

    this.postService.likePost(post.id, this.currentUserId).subscribe({
      next: () => {
        post.likesCount += 1;
      },
      error: () => {
      }
    });
  }

  toggleComments(post: PostViewModel): void {
    post.commentsOpen = !post.commentsOpen;

    if (post.commentsOpen && !post.commentsLoaded) {
      this.loadComments(post);
    }
  }

  loadComments(post: PostViewModel): void {
    post.commentsLoading = true;
    post.commentsError = '';

    this.postService.getComments(post.id).subscribe({
      next: async (comments) => {
        const uniqueUserIds = [...new Set(comments.map((comment) => comment.userId))];

        const profileRequests = uniqueUserIds.map((userId) =>
          this.profileService.getProfileByUserId(userId).toPromise().then(
            (profile) => ({ userId, profile }),
            () => ({ userId, profile: null as ProfileResponse | null })
          )
        );

        const results = await Promise.all(profileRequests);
        const profileMap = new Map<string, ProfileResponse | null>();

        results.forEach((result) => {
          profileMap.set(result.userId, result.profile ?? null);
        });

        post.comments = comments.map((comment) => {
          const profile = profileMap.get(comment.userId) ?? null;

          return {
            ...comment,
            authorName: profile?.fullName ?? 'Unknown Creator',
            username: profile?.username ?? 'unknown'
          };
        });

        post.commentsLoaded = true;
        post.commentsLoading = false;
      },
      error: () => {
        post.commentsError = 'Failed to load comments.';
        post.commentsLoading = false;
      }
    });
  }

  submitComment(post: PostViewModel): void {
    const content = post.newComment.trim();

    if (!this.currentUserId || !content) return;

    this.postService.addComment(post.id, this.currentUserId, content).subscribe({
      next: async (comment) => {
        let profile: ProfileResponse | null = null;

        try {
          profile = await this.profileService.getProfileByUserId(comment.userId).toPromise() ?? null;
        } catch {
          profile = null;
        }

        post.comments.unshift({
          ...comment,
          authorName: profile?.fullName ?? 'You',
          username: profile?.username ?? this.currentUsername
        });

        post.commentsCount += 1;
        post.newComment = '';
        post.commentsOpen = true;
        post.commentsLoaded = true;
      },
      error: () => {
        post.commentsError = 'Failed to add comment.';
      }
    });
  }

  clearPreviewUrls(): void {
    this.previewUrls.forEach((url) => URL.revokeObjectURL(url));
    this.previewUrls = [];
  }

  isImageFile(file: File): boolean {
    return file.type.startsWith('image/');
  }

  isVideoFile(file: File): boolean {
    return file.type.startsWith('video/');
  }

  getPreviewGridClass(count: number): string {
    if (count === 1) return 'single';
    if (count === 2) return 'double';
    if (count === 3) return 'triple';
    return 'quad';
  }

  trackByPostId(index: number, post: PostViewModel): string {
    return post.id;
  }
}