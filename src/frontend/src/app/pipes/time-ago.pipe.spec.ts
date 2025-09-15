import { TimeAgoPipe } from './time-ago.pipe';

describe('TimeAgoPipe', () => {
  const pipe = new TimeAgoPipe();

  it('shows minutes ago for recent timestamps', () => {
    const nowSecs = Math.floor(Date.now() / 1000);
    const fiveMinAgo = nowSecs - 5 * 60;
    const result = pipe.transform(fiveMinAgo);
    expect(result).toContain('minute');
  });

  it('handles future timestamp gracefully', () => {
    const nowSecs = Math.floor(Date.now() / 1000);
    const future = nowSecs + 60;
    const result = pipe.transform(future);
    expect(typeof result).toBe('string');
  });
});
