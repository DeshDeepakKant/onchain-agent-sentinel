import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { HackerNewsService } from './hacker-news.service';
import { PagedResult } from '../models/paged-result.model';
import { Story } from '../models/story.model';

describe('HackerNewsService (simple)', () => {
  let service: HackerNewsService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [provideZonelessChangeDetection(), HackerNewsService],
    });

    service = TestBed.inject(HackerNewsService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getStories() makes a GET with category/page/pageSize', () => {
    const mockResult: PagedResult<Story> = {
      items: [{ id: 1, title: 'Test Story', type: 'story', score: 10, time: 0, commentCount: 0 }],
      page: 1,
      pageSize: 30,
      totalCount: 1,
      hasMore: false,
    };

    service.getStories('top', 1, 30).subscribe((result) => {
      expect(result).toEqual(mockResult);
    });

    const req = httpMock.expectOne(
      (r) =>
        r.url.endsWith('/api/stories') &&
        r.params.get('category') === 'top' &&
        r.params.get('page') === '1' &&
        r.params.get('pageSize') === '30'
    );
    expect(req.request.method).toBe('GET');
    req.flush(mockResult);
  });

  it('searchStories() hits /search with q param', () => {
    const mockResult: PagedResult<Story> = {
      items: [{ id: 2, title: 'Search Story', type: 'story', score: 5, time: 0, commentCount: 0 }],
      page: 2,
      pageSize: 30,
      totalCount: 40,
      hasMore: true,
    };

    service.searchStories('new', 'angular', 2, 30).subscribe((result) => {
      expect(result.items.length).toBe(1);
      expect(result.items[0].title).toBe('Search Story');
    });

    const req = httpMock.expectOne(
      (r) =>
        r.url.endsWith('/api/stories/search') &&
        r.params.get('category') === 'new' &&
        r.params.get('q') === 'angular' &&
        r.params.get('page') === '2' &&
        r.params.get('pageSize') === '30'
    );
    expect(req.request.method).toBe('GET');
    req.flush(mockResult);
  });
});
