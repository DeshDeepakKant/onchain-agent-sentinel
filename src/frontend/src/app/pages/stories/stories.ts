import { Component, effect, inject, input, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs';
import { StoryListItem } from '../../components/story-list-item/story-list-item';
import { HackerNewsService } from '../../services/hacker-news.service';
import { Story } from '../../models/story.model';

@Component({
  selector: 'app-stories',
  standalone: true,
  imports: [StoryListItem],
  templateUrl: './stories.html',
  styleUrl: './stories.scss',
})
export class Stories {
  category = input.required<string>();

  stories = signal<Story[]>([]);
  currentPage = signal(1);
  hasMore = signal(false);
  isLoading = signal(true);
  searchQuery = signal('');

  private hnService = inject(HackerNewsService);

  private searchQuery$ = toObservable(this.searchQuery).pipe(
    debounceTime(300),
    distinctUntilChanged()
  );

  private searchResults = toSignal(
    this.searchQuery$.pipe(
      switchMap((query) => {
        this.isLoading.set(true);
        this.currentPage.set(1);
        this.stories.set([]);
        if (!query) {
          return this.hnService.getStories(this.category(), 1);
        }
        return this.hnService.searchStories(query, 1);
      })
    )
  );

  constructor() {
    effect(
      () => {
        console.log('effect fired');
        this.isLoading.set(true);
        this.currentPage.set(1);
        this.stories.set([]);
        this.hnService.getStories(this.category(), 1).subscribe((result) => {
          this.stories.set(result.items);
          this.hasMore.set(result.hasMore);
          this.isLoading.set(false);
        });
      },
      { allowSignalWrites: true }
    );

    effect(() => {
      const result = this.searchResults();
      if (result) {
        this.stories.set(result.items);
        this.hasMore.set(result.hasMore);
        this.isLoading.set(false);
      }
    });
  }

  loadMore(): void {
    if (!this.hasMore() || this.isLoading()) return;

    this.isLoading.set(true);
    const nextPage = this.currentPage() + 1;
    const query = this.searchQuery();

    const apiCall = query
      ? this.hnService.searchStories(query, nextPage)
      : this.hnService.getStories(this.category(), nextPage);

    apiCall.subscribe((result) => {
      this.stories.update((currentStories) => [...currentStories, ...result.items]);
      this.hasMore.set(result.hasMore);
      this.currentPage.set(nextPage);
      this.isLoading.set(false);
    });
  }

  onSearch(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.searchQuery.set(input.value);
  }
}
