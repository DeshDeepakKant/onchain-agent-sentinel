import { DomainPipe } from './domain.pipe';

describe('DomainPipe', () => {
  const pipe = new DomainPipe();

  it('extracts host from URL', () => {
    expect(pipe.transform('https://sub.example.com/path?q=1')).toBe('sub.example.com');
  });

  it('returns empty string when url is falsy', () => {
    expect(pipe.transform('')).toBe('');
    // @ts-expect-error
    expect(pipe.transform(null)).toBe('');
  });
});
