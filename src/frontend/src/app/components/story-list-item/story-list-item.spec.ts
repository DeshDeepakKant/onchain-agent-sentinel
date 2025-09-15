import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { StoryListItem } from './story-list-item';
import { Story } from '../../models/story.model';

describe('StoryListItem link fallback', () => {
  let fixture: ComponentFixture<StoryListItem>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StoryListItem, RouterTestingModule],
      providers: [provideZonelessChangeDetection()],
    }).compileComponents();

    fixture = TestBed.createComponent(StoryListItem);
  });

  it('uses HN item link when url is missing', () => {
    const story: Story = {
      id: 123,
      title: 'No URL Story',
      type: 'story',
      score: 10,
      time: 0,
      commentCount: 0,
    };

    fixture.componentRef.setInput('rank', 1);
    fixture.componentRef.setInput('story', story);

    fixture.detectChanges();

    const anchor: HTMLAnchorElement | null = fixture.nativeElement.querySelector('.title');
    expect(anchor).toBeTruthy();
    expect(anchor!.getAttribute('href')).toBe('https://news.ycombinator.com/item?id=123');
  });
});
