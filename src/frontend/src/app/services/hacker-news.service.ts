import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedResult } from '../models/paged-result.model';
import { Story } from '../models/story.model';
import { Environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class HackerNewsService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${Environment.apiUrl}/api/stories`;

  getStories(
    category: string,
    page: number = 1,
    pageSize: number = 30
  ): Observable<PagedResult<Story>> {
    const params = new HttpParams()
      .set('category', category)
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<PagedResult<Story>>(this.baseUrl, { params });
  }

  searchStories(
    category: string,
    query: string,
    page: number = 1,
    pageSize: number = 30
  ): Observable<PagedResult<Story>> {
    const params = new HttpParams()
      .set('category', category)
      .set('q', query)
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<PagedResult<Story>>(`${this.baseUrl}/search`, { params });
  }
}
