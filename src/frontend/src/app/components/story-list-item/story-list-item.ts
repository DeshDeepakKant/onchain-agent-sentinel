import { Component, input } from '@angular/core';
import { Story } from '../../models/story.model';
import { DomainPipe } from '../../pipes/domain.pipe';
import { TimeAgoPipe } from '../../pipes/time-ago.pipe';

@Component({
  selector: 'tr[app-story-list-item]',
  standalone: true,
  templateUrl: './story-list-item.html',
  styleUrl: './story-list-item.scss',
  imports: [DomainPipe, TimeAgoPipe],
})
export class StoryListItem {
  story = input.required<Story>();
  rank = input.required<number>();
}
