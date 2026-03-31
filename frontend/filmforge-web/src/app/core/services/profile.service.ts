import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface CreateProfileRequest {
  userId: string;
  fullName: string;
  username: string;
  bio?: string;
  primaryCraft: string;
  secondaryCrafts?: string;
  location?: string;
  experienceLevel?: string;
  availabilityStatus?: string;
  portfolioUrl?: string;
  instagramUrl?: string;
  youtubeUrl?: string;
  profileImageUrl?: string;
}

export interface ProfileResponse {
  id: string;
  userId: string;
  fullName: string;
  username: string;
  bio?: string;
  primaryCraft: string;
  secondaryCrafts?: string;
  location?: string;
  experienceLevel?: string;
  availabilityStatus?: string;
  portfolioUrl?: string;
  instagramUrl?: string;
  youtubeUrl?: string;
  profileImageUrl?: string;
  createdAt: string;
  updatedAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private http = inject(HttpClient);
  private apiBaseUrl = 'http://localhost:5156/api/createprofile';

  createProfile(payload: CreateProfileRequest): Observable<ProfileResponse> {
    return this.http.post<ProfileResponse>(this.apiBaseUrl, payload);
  }

  updateProfile(userId: string, payload: CreateProfileRequest): Observable<ProfileResponse> {
    return this.http.put<ProfileResponse>(`${this.apiBaseUrl}/${userId}`, payload);
  }

  uploadProfileImage(userId: string, file: File): Observable<ProfileResponse> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.put<ProfileResponse>(`${this.apiBaseUrl}/${userId}/image`, formData);
  }

  getProfileByUsername(username: string): Observable<ProfileResponse> {
    return this.http.get<ProfileResponse>(`${this.apiBaseUrl}/${username}`);
  }

  getProfileByUserId(userId: string): Observable<ProfileResponse> {
    return this.http.get<ProfileResponse>(`${this.apiBaseUrl}/by-userid/${userId}`);
  }
  getAllProfiles() {
    return this.http.get<ProfileResponse[]>(this.apiBaseUrl);
  }
  
}