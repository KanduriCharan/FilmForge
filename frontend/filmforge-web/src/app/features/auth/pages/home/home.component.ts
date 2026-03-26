import {
  AfterViewInit,
  Component,
  ElementRef,
  HostListener,
  OnDestroy,
  QueryList,
  ViewChild,
  ViewChildren
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

interface StoryStep {
  eyebrow: string;
  title: string;
  description: string;
  accent: string;
}

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements AfterViewInit, OnDestroy {
  @ViewChildren('storyStep') storyStepElements!: QueryList<ElementRef<HTMLElement>>;
  @ViewChild('storySection') storySectionRef!: ElementRef<HTMLElement>;

  activeStepIndex = 0;
  visualScale = 0.92;
  visualTranslateY = 0;
  visualRotate = -8;

  private observer?: IntersectionObserver;

  storySteps: StoryStep[] = [
    {
      eyebrow: 'Learn the craft',
      title: 'Explore the 24 crafts that bring cinema to life.',
      description:
        'From directing and cinematography to editing, sound, VFX, costume, production design, and writing — FilmForge helps creators understand every department of filmmaking.',
      accent: 'crafts'
    },
    {
      eyebrow: 'Find your crew',
      title: 'Turn ideas into projects with the right collaborators.',
      description:
        'Meet writers, editors, cinematographers, actors, composers, and producers who are actively looking to join serious creative work.',
      accent: 'crew'
    },
    {
      eyebrow: 'Showcase your work',
      title: 'Present your reels, shorts, scripts, and creative identity.',
      description:
        'Build a profile that feels like a premium portfolio — not just a social account. Let people discover your taste, voice, and craft.',
      accent: 'showcase'
    },
    {
      eyebrow: 'Write and protect',
      title: 'Create scripts, revise drafts, and protect your ideas.',
      description:
        'Collaborative writing tools, version history, comments, and watermarking make FilmForge more than a social network — it becomes part of your production workflow.',
      accent: 'scripts'
    }
  ];

  crafts = [
    'Directing',
    'Screenwriting',
    'Cinematography',
    'Editing',
    'Sound Design',
    'Acting',
    'Production Design',
    'VFX'
  ];

  ngAfterViewInit(): void {
    this.observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          const index = Number((entry.target as HTMLElement).dataset['index']);
          if (entry.isIntersecting) {
            this.activeStepIndex = index;
          }
        });
      },
      {
        threshold: 0.58
      }
    );

    this.storyStepElements.forEach((step, index) => {
      step.nativeElement.dataset['index'] = String(index);
      this.observer?.observe(step.nativeElement);
    });

    this.updateStoryVisual();
  }

  @HostListener('window:scroll')
  @HostListener('window:resize')
  updateStoryVisual(): void {
    if (!this.storySectionRef) return;

    const rect = this.storySectionRef.nativeElement.getBoundingClientRect();
    const viewportHeight = window.innerHeight;

    const start = viewportHeight * 0.75;
    const end = rect.height - viewportHeight * 0.35;
    const rawProgress = (start - rect.top) / Math.max(end, 1);
    const progress = Math.max(0, Math.min(1, rawProgress));

    this.visualScale = 0.92 + progress * 0.28;
    this.visualTranslateY = progress * 48;
    this.visualRotate = -8 + progress * 8;
  }

  ngOnDestroy(): void {
    this.observer?.disconnect();
  }
}