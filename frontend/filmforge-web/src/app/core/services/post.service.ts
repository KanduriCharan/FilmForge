import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface PostMediaResponse {
  id: string;
  mediaUrl: string;
  mediaType: string;
  sortOrder: number;
}

export interface PostResponse {
  id: string;
  userId: string;
  caption: string;
  createdAt: string;
  updatedAt: string;
  likesCount: number;
  commentsCount: number;
  mediaItems: PostMediaResponse[];
}

export interface PostCommentResponse {
  id: string;
  postId: string;
  userId: string;
  content: string;
  createdAt: string;
  updatedAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class PostService {
  private http = inject(HttpClient);
  private apiBaseUrl = 'http://localhost:5156/api/posts';

  createPost(userId: string, caption: string, files: File[]): Observable<PostResponse> {
    const formData = new FormData();
    formData.append('userId', userId);
    formData.append('caption', caption);

    files.forEach((file) => {
      formData.append('files', file);
    });

    return this.http.post<PostResponse>(this.apiBaseUrl, formData);
  }

  getPosts(): Observable<PostResponse[]> {
    return this.http.get<PostResponse[]>(this.apiBaseUrl);
  }

  likePost(postId: string, userId: string): Observable<any> {
    const params = new HttpParams().set('userId', userId);
    return this.http.post(`${this.apiBaseUrl}/${postId}/like`, null, { params });
  }

  unlikePost(postId: string, userId: string): Observable<any> {
    const params = new HttpParams().set('userId', userId);
    return this.http.delete(`${this.apiBaseUrl}/${postId}/like`, { params });
  }

  getComments(postId: string): Observable<PostCommentResponse[]> {
    return this.http.get<PostCommentResponse[]>(`${this.apiBaseUrl}/${postId}/comments`);
  }

  addComment(postId: string, userId: string, content: string): Observable<PostCommentResponse> {
    return this.http.post<PostCommentResponse>(`${this.apiBaseUrl}/${postId}/comments`, {
      userId,
      content
    });
  }
}