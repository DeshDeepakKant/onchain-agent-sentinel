import { Routes } from '@angular/router';
import { Stories } from './pages/stories/stories';

export const routes: Routes = [
  { path: '', component: Stories, data: { category: 'new' }, title: 'Hacker News Clone' },
  { path: 'top', component: Stories, data: { category: 'top' }, title: 'Top Stories' },
  { path: 'best', component: Stories, data: { category: 'best' }, title: 'Best Stories' },
  { path: 'ask', component: Stories, data: { category: 'ask' }, title: 'Ask HN' },
  { path: 'show', component: Stories, data: { category: 'show' }, title: 'Show HN' },
  { path: 'jobs', component: Stories, data: { category: 'job' }, title: 'Jobs' },
  { path: '**', redirectTo: '', pathMatch: 'full' },
];
