import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StoryListItem } from './story-list-item';

describe('StoryListItem', () => {
  let component: StoryListItem;
  let fixture: ComponentFixture<StoryListItem>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StoryListItem]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StoryListItem);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
