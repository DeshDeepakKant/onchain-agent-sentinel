import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'domain',
  standalone: true,
})
export class DomainPipe implements PipeTransform {
  transform(url: string | undefined): string {
    if (!url) {
      return '';
    }
    try {
      const hostname = new URL(url).hostname;
      return `${hostname.replace(/^www\./, '')}`;
    } catch (e) {
      return '';
    }
  }
}
