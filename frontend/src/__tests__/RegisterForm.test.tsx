import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MemoryRouter } from 'react-router-dom';
import '../i18n/index';
import RegisterPage from '../pages/auth/Register';

// Provide a safe no-op AuthProvider/useAuth for these tests
jest.mock('../hooks/useAuth', () => ({
  useAuth: () => ({
    user: null,
    householdId: null,
    loading: false,
    login: jest.fn(),
    register: jest.fn().mockRejectedValue(new Error('mock')),
    logout: jest.fn(),
  }),
  AuthProvider: ({ children }: { children: React.ReactNode }) => <>{children}</>,
}));

function renderRegister() {
  return render(
    <MemoryRouter>
      <RegisterPage />
    </MemoryRouter>,
  );
}

describe('Register form validation', () => {
  it('shows validation error on empty email when form is submitted', async () => {
    const user = userEvent.setup();
    renderRegister();

    // Fill in a display name and password but leave email empty
    await user.type(screen.getByLabelText(/display name/i), 'Test User');
    await user.type(screen.getByLabelText(/password/i), 'password123');

    // Submit without email
    await user.click(screen.getByRole('button', { name: /sign up/i }));

    // Should show "required" error
    expect(screen.getByText(/required/i)).toBeInTheDocument();
  });

  it('shows validation error on invalid email format (abc@)', async () => {
    const user = userEvent.setup();
    renderRegister();

    // Fill all fields but with a bad email
    await user.type(screen.getByLabelText(/display name/i), 'Test User');
    await user.type(screen.getByLabelText(/email/i), 'abc@');
    await user.type(screen.getByLabelText(/password/i), 'password123');

    await user.click(screen.getByRole('button', { name: /sign up/i }));

    // Should show invalid email message
    expect(screen.getByText(/valid email/i)).toBeInTheDocument();
  });
});
