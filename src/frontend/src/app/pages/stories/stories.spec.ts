import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { of } from 'rxjs';

import { Stories } from './stories';
import { HackerNewsService } from '../../services/hacker-news.service';
import { PagedResult } from '../../models/paged-result.model';
import { Story } from '../../models/story.model';

const mockService = {
  getStories: jasmine.createSpy('getStories').and.returnValue(
    of({
      items: [{ id: 1, title: 'Mock Story', type: 'story', score: 10, time: 0, commentCount: 0 }],
      page: 1,
      pageSize: 30,
      totalCount: 1,
      hasMore: false,
    } as PagedResult<Story>)
  ),
  searchStories: jasmine.createSpy('searchStories').and.returnValue(
    of({
      items: [],
      page: 1,
      pageSize: 30,
      totalCount: 0,
      hasMore: false,
    } as PagedResult<Story>)
  ),
};

describe('Stories (simple)', () => {
  let fixture: ComponentFixture<Stories>;
  let component: Stories;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Stories, RouterTestingModule],
      providers: [
        provideZonelessChangeDetection(),
        { provide: HackerNewsService, useValue: mockService },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: { data: { category: 'new' } },
            queryParamMap: of(convertToParamMap({})),
          },
        },
      ],
    }).compileComponents();

    spyOn(window, 'scrollTo').and.returnValue();

    fixture = TestBed.createComponent(Stories);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('loads stories on init using the route category', () => {
    fixture.detectChanges();

    expect(mockService.getStories).toHaveBeenCalledWith('new', 1);
    expect(mockService.getStories.calls.mostRecent().args).toEqual(['new', 1]);
    expect(component.stories().length).toBe(1);
    expect(component.stories()[0].title).toBe('Mock Story');
    expect(component.hasMore()).toBeFalse();
  });
});
