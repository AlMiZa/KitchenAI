import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import i18n from '../i18n/index';
import CookieBanner from '../components/CookieBanner';

beforeAll(async () => { await i18n.changeLanguage('en'); });
afterAll(async () => { await i18n.changeLanguage('pl'); });

beforeEach(() => {
  localStorage.clear();
});

describe('CookieBanner', () => {
  it('renders when no consent is stored', () => {
    render(<CookieBanner />);
    expect(screen.getByRole('dialog')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /accept all/i })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /decline/i })).toBeInTheDocument();
  });

  it('does not render after consent is already accepted', () => {
    localStorage.setItem('cookieConsent', 'accepted');
    render(<CookieBanner />);
    expect(screen.queryByRole('dialog')).not.toBeInTheDocument();
  });

  it('does not render after consent is already declined', () => {
    localStorage.setItem('cookieConsent', 'declined');
    render(<CookieBanner />);
    expect(screen.queryByRole('dialog')).not.toBeInTheDocument();
  });

  it('hides banner and stores accepted when Accept is clicked', async () => {
    const user = userEvent.setup();
    render(<CookieBanner />);
    await user.click(screen.getByRole('button', { name: /accept all/i }));
    expect(screen.queryByRole('dialog')).not.toBeInTheDocument();
    expect(localStorage.getItem('cookieConsent')).toBe('accepted');
  });

  it('hides banner and stores declined when Decline is clicked', async () => {
    const user = userEvent.setup();
    render(<CookieBanner />);
    await user.click(screen.getByRole('button', { name: /decline/i }));
    expect(screen.queryByRole('dialog')).not.toBeInTheDocument();
    expect(localStorage.getItem('cookieConsent')).toBe('declined');
  });

  it('dialog has aria-modal attribute for accessibility', () => {
    render(<CookieBanner />);
    const dialog = screen.getByRole('dialog');
    expect(dialog).toHaveAttribute('aria-modal', 'true');
  });
});
