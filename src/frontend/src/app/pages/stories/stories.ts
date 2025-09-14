import { Component, inject, signal } from '@angular/core';
import { toObservable } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { debounceTime, distinctUntilChanged, switchMap, tap } from 'rxjs';
import { StoryListItem } from '../../components/story-list-item/story-list-item';
import { HackerNewsService } from '../../services/hacker-news.service';
import { Story } from '../../models/story.model';
import { SearchBar } from '../../components/search-bar/search-bar';

@Component({
  selector: 'app-stories',
  standalone: true,
  imports: [StoryListItem, SearchBar, RouterLink],
  templateUrl: './stories.html',
  styleUrl: './stories.scss',
})
export class Stories {
  stories = signal<Story[]>([]);
  currentPage = signal(1);
  hasMore = signal(false);
  isLoading = signal(true);
  searchQuery = signal('');

  private hnService = inject(HackerNewsService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  private data$ = this.route.queryParamMap.pipe(
    switchMap((queryParams) => {
      const category = this.route.snapshot.data['category'];
      const page = Number(queryParams.get('p') ?? '1');
      const query = queryParams.get('q') ?? '';

      this.isLoading.set(true);
      this.currentPage.set(page);
      this.searchQuery.set(query);

      const apiCall = query
        ? this.hnService.searchStories(category, query, page)
        : this.hnService.getStories(category, page);

      return apiCall.pipe(
        tap((result) => {
          this.stories.set(result.items);
          this.hasMore.set(result.hasMore);
          this.isLoading.set(false);
          window.scrollTo(0, 0);
        })
      );
    })
  );

  constructor() {
    toObservable(this.searchQuery)
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe((query) => {
        this.router.navigate([], {
          relativeTo: this.route,
          queryParams: { q: query || null, p: null },
          queryParamsHandling: 'merge',
        });
      });

    this.data$.subscribe();
  }
}
