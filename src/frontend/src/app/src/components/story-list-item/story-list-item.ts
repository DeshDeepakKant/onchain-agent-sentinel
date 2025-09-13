import { Component, input } from '@angular/core';
import { Story } from '../../../models/story.model';

@Component({
  selector: 'tr[app-story-list-item]',
  standalone: true,
  templateUrl: './story-list-item.html',
  styleUrl: './story-list-item.scss',
})
export class StoryListItem {
  story = input.required<Story>();
  rank = input.required<number>();
}
