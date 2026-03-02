import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../../hooks/useAuth';
import { sendMagicLink } from '../../services/auth';

const EMAIL_REGEX = /^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/;

interface FormErrors {
  email?: string;
  password?: string;
  general?: string;
}

export default function LoginPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const { login } = useAuth();

  const [email, setEmail]       = useState('');
  const [password, setPassword] = useState('');
  const [errors, setErrors]     = useState<FormErrors>({});
  const [loading, setLoading]   = useState(false);
  const [magicLinkSent, setMagicLinkSent] = useState(false);
  const [magicLinkLoading, setMagicLinkLoading] = useState(false);

  const validate = (): boolean => {
    const errs: FormErrors = {};
    if (!email.trim())             errs.email = t('auth.required');
    else if (!EMAIL_REGEX.test(email)) errs.email = t('auth.invalidEmail');
    if (!password)                 errs.password = t('auth.required');
    else if (password.length < 8) errs.password = t('auth.passwordTooShort');
    setErrors(errs);
    return Object.keys(errs).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;
    setLoading(true);
    try {
      await login({ email, password });
      navigate('/dashboard');
    } catch {
      setErrors({ general: t('auth.invalidCredentials') });
    } finally {
      setLoading(false);
    }
  };

  const handleMagicLink = async () => {
    if (!email.trim()) {
      setErrors((p) => ({ ...p, email: t('auth.required') }));
      return;
    }
    if (!EMAIL_REGEX.test(email)) {
      setErrors((p) => ({ ...p, email: t('auth.invalidEmail') }));
      return;
    }
    setMagicLinkLoading(true);
    try {
      await sendMagicLink(email);
      setMagicLinkSent(true);
    } catch {
      setErrors((p) => ({ ...p, general: t('auth.magicLinkError') }));
    } finally {
      setMagicLinkLoading(false);
    }
  };

  if (magicLinkSent) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50 px-4">
        <div className="w-full max-w-sm bg-white rounded-2xl shadow-md p-8 space-y-6 text-center">
          <h1 className="text-2xl font-bold text-green-600">KitchenAI</h1>
          <div className="text-5xl">📬</div>
          <h2 className="text-lg font-semibold text-gray-800">{t('auth.checkYourEmail')}</h2>
          <p className="text-sm text-gray-500">{t('auth.magicLinkSent', { email })}</p>
          <button
            onClick={() => setMagicLinkSent(false)}
            className="text-green-600 hover:underline text-sm"
          >
            {t('auth.backToLogin')}
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 px-4">
      <div className="w-full max-w-sm bg-white rounded-2xl shadow-md p-8 space-y-6">
        <div className="text-center">
          <h1 className="text-2xl font-bold text-green-600">KitchenAI</h1>
          <p className="mt-1 text-gray-500 text-sm">{t('auth.login')}</p>
        </div>

        {errors.general && (
          <p role="alert" className="text-red-600 text-sm text-center bg-red-50 rounded-lg p-2">
            {errors.general}
          </p>
        )}

        <form onSubmit={handleSubmit} noValidate className="space-y-4">
          {/* Email */}
          <div>
            <label htmlFor="login-email" className="block text-sm font-medium text-gray-700 mb-1">
              {t('auth.email')}
            </label>
            <input
              id="login-email"
              type="email"
              autoComplete="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              onBlur={() => {
                if (!email.trim()) setErrors((p) => ({ ...p, email: t('auth.required') }));
                else if (!EMAIL_REGEX.test(email))
                  setErrors((p) => ({ ...p, email: t('auth.invalidEmail') }));
                else setErrors((p) => ({ ...p, email: undefined }));
              }}
              aria-describedby={errors.email ? 'login-email-error' : undefined}
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500"
            />
            {errors.email && (
              <p id="login-email-error" className="text-red-500 text-xs mt-1">{errors.email}</p>
            )}
          </div>

          {/* Password */}
          <div>
            <label htmlFor="login-password" className="block text-sm font-medium text-gray-700 mb-1">
              {t('auth.password')}
            </label>
            <input
              id="login-password"
              type="password"
              autoComplete="current-password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              aria-describedby={errors.password ? 'login-password-error' : undefined}
              className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500"
            />
            {errors.password && (
              <p id="login-password-error" className="text-red-500 text-xs mt-1">{errors.password}</p>
            )}
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full bg-green-600 hover:bg-green-700 disabled:opacity-50 text-white py-2.5 rounded-lg font-medium text-sm transition-colors"
          >
            {loading ? t('common.loading') : t('auth.login')}
          </button>
        </form>

        <div className="relative flex items-center">
          <div className="flex-grow border-t border-gray-200" />
          <span className="mx-3 text-xs text-gray-400">{t('auth.or')}</span>
          <div className="flex-grow border-t border-gray-200" />
        </div>

        <button
          type="button"
          disabled={magicLinkLoading}
          onClick={handleMagicLink}
          className="w-full border border-gray-300 hover:border-green-500 disabled:opacity-50 text-gray-700 py-2.5 rounded-lg font-medium text-sm transition-colors"
        >
          {magicLinkLoading ? t('common.loading') : t('auth.sendMagicLink')}
        </button>

        <p className="text-center text-sm text-gray-500">
          <Link to="/register" className="text-green-600 hover:underline">
            {t('auth.dontHaveAccount')}
          </Link>
        </p>
      </div>
    </div>
  );
}
