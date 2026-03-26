import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/pages/login/login.component';
import { SignupComponent } from './features/auth/pages/signup/signup.component';
import { HomeComponent } from './features/auth/pages/home/home.component';
import { OnboardingComponent } from './features/profile/pages/onboarding/onboarding.component';
import { ProfileViewComponent } from './features/profile/pages/profile-view/profile-view.component';
import { StudioComponent } from './features/Pages/studio/studio.component';

export const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  {path: 'home', component: HomeComponent},
  { path: 'login', component: LoginComponent },
  { path: 'signup', component: SignupComponent },
  {path: 'onboarding', component: OnboardingComponent},
  {path: 'profile/:username', component: ProfileViewComponent},
  {path: 'studio', component: StudioComponent}


];