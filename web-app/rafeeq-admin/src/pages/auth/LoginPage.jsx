import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { login as loginRequest } from "../../api/authApi";
import { setAuthState } from "../../auth/authState";
import "./LoginPage.css";

// ── Icons (inline SVG, no deps) ──────────────────────────────
const IconMail = () => (
  <svg width="17" height="17" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" strokeLinejoin="round">
    <rect x="2" y="4" width="20" height="16" rx="2"/><polyline points="2,4 12,13 22,4"/>
  </svg>
);
const IconLock = () => (
  <svg width="17" height="17" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" strokeLinejoin="round">
    <rect x="3" y="11" width="18" height="11" rx="2"/><path d="M7 11V7a5 5 0 0 1 10 0v4"/>
  </svg>
);
const IconEye = () => (
  <svg width="17" height="17" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" strokeLinejoin="round">
    <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/><circle cx="12" cy="12" r="3"/>
  </svg>
);
const IconEyeOff = () => (
  <svg width="17" height="17" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" strokeLinejoin="round">
    <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94"/><path d="M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19"/><line x1="1" y1="1" x2="23" y2="23"/>
  </svg>
);
const IconAlert = () => (
  <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/>
  </svg>
);
const IconCheck = () => (
  <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <polyline points="20 6 9 17 4 12"/>
  </svg>
);
const IconArrow = () => (
  <svg width="17" height="17" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.2" strokeLinecap="round" strokeLinejoin="round">
    <line x1="5" y1="12" x2="19" y2="12"/><polyline points="12 5 19 12 12 19"/>
  </svg>
);
const IconCompass = () => (
  <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="rgba(255,220,188,0.85)" strokeWidth="1.6" strokeLinecap="round" strokeLinejoin="round">
    <circle cx="12" cy="12" r="10"/><polygon points="16.24,7.76 14.12,14.12 7.76,16.24 9.88,9.88" fill="rgba(255,220,188,0.3)"/>
  </svg>
);

// ── Main Component ───────────────────────────────────────────
export default function LoginPage() {
  const navigate = useNavigate();

  const [email, setEmail]         = useState("");
  const [password, setPassword]   = useState("");
  const [showPw, setShowPw]       = useState(false);
  const [remember, setRemember]   = useState(false);
  const [loading, setLoading]     = useState(false);
  const [alert, setAlert]         = useState(null); // { type: 'error'|'success', msg }
  const [errors, setErrors]       = useState({});

  const validate = () => {
    const e = {};
    if (!email.trim())               e.email = "Email is required.";
    else if (!/\S+@\S+\.\S+/.test(email)) e.email = "Enter a valid email address.";
    if (!password)                   e.password = "Password is required.";
    else if (password.length < 6)    e.password = "Password must be at least 6 characters.";
    return e;
  };

  const handleSubmit = async () => {
    if (loading) return;

    setAlert(null);
    const e = validate();
    if (Object.keys(e).length) { setErrors(e); return; }
    setErrors({});
    setLoading(true);

    try {
      await loginRequest({ email: email.trim(), password });

      // Cookies are now set; mark session as authenticated in-memory.
      setAuthState({ isAuthenticated: true, hasChecked: true });

      // Access & Refresh tokens are set as HTTP-only cookies by the server.
      // No token handling needed on the client side.
      setAlert({ type: "success", msg: "Logged in successfully. Redirecting…" });
      setTimeout(() => {
        navigate("/dashboard", { replace: true });
      }, 1200);
    } catch (err) {
      const msg =
        err?.response?.data?.detail ||
        err?.response?.data?.message ||
        "Invalid credentials. Please try again.";

      setAlert({ type: "error", msg });
    } finally {
      setLoading(false);
    }
  };

  const handleKey = (e) => {
    if (e.key === "Enter") {
      e.preventDefault();
      handleSubmit();
    }
  };

  return (
    <div className="login-root">

        {/* ── Left decorative panel ── */}
        <div className="login-panel">
          <div className="panel-noise" />
          <svg className="panel-rings" viewBox="0 0 520 520" xmlns="http://www.w3.org/2000/svg">
            <circle cx="260" cy="260" r="200"/>
            <circle cx="260" cy="260" r="280"/>
            <circle cx="260" cy="260" r="360"/>
          </svg>

          <div className="panel-top">
            <div className="logo-mark">
              <div className="logo-icon"><IconCompass /></div>
              <span className="logo-text">Rafeeq<span> Admin</span></span>
            </div>
          </div>

          <div className="panel-middle">
            <div className="panel-tag">
              <span className="panel-tag-dot" />
              Tourism Management
            </div>
            <h1 className="panel-headline">
              Discover<br />Egypt's <em>hidden</em><br />wonders.
            </h1>
            <p className="panel-subtext">
              Manage destinations, curate experiences, and connect travellers to the richness of Egyptian heritage.
            </p>
          </div>

          <div className="panel-stats">
            {/* <div className="stat-item">
              <span className="stat-value">120+</span>
              <span className="stat-label">Destinations</span>
            </div>
            <div className="stat-item">
              <span className="stat-value">40k</span>
              <span className="stat-label">Active users</span>
            </div>
            <div className="stat-item">
              <span className="stat-value">5★</span>
              <span className="stat-label">Avg. rating</span>
            </div> */}
          </div>
        </div>

        {/* ── Right form panel ── */}
        <div className="login-form-side">
          <div className="login-box">

            <div className="login-header">
              <p className="login-eyebrow">Admin Portal</p>
              <h2 className="login-title">Welcome back</h2>
              <p className="login-subtitle">Sign in to manage the Rafeeq platform.</p>
            </div>

            {alert && (
              <div className={`alert alert-${alert.type}`}>
                {alert.type === "error" ? <IconAlert /> : <IconCheck />}
                {alert.msg}
              </div>
            )}

            {/* Email */}
            <div className="field">
              <label htmlFor="rf-email">Email address</label>
              <div className="input-wrap">
                <span className="input-icon"><IconMail /></span>
                <input
                  id="rf-email"
                  type="email"
                  placeholder="admin@rafeeq.com"
                  value={email}
                  onChange={e => setEmail(e.target.value)}
                  onKeyDown={handleKey}
                  className={errors.email ? "error-input" : ""}
                  autoComplete="email"
                  disabled={loading}
                />
              </div>
              {errors.email && (
                <p className="field-error"><IconAlert />{errors.email}</p>
              )}
            </div>

            {/* Password */}
            <div className="field">
              <label htmlFor="rf-password">Password</label>
              <div className="input-wrap">
                <span className="input-icon"><IconLock /></span>
                <input
                  id="rf-password"
                  type={showPw ? "text" : "password"}
                  placeholder="Enter your password"
                  value={password}
                  onChange={e => setPassword(e.target.value)}
                  onKeyDown={handleKey}
                  className={errors.password ? "error-input" : ""}
                  autoComplete="current-password"
                  disabled={loading}
                />
                <button
                  type="button"
                  className="toggle-pw"
                  onClick={() => setShowPw(v => !v)}
                  aria-label={showPw ? "Hide password" : "Show password"}
                >
                  {showPw ? <IconEyeOff /> : <IconEye />}
                </button>
              </div>
              {errors.password && (
                <p className="field-error"><IconAlert />{errors.password}</p>
              )}
            </div>

            {/* Options row */}
            <div className="row-options">
              <label className="remember-label">
                <input
                  type="checkbox"
                  checked={remember}
                  onChange={e => setRemember(e.target.checked)}
                  disabled={loading}
                />
                Remember me
              </label>
              <a href="/forgot-password" className="forgot-link">Forgot password?</a>
            </div>

            {/* Submit */}
            <button
              className="btn-login"
              onClick={handleSubmit}
              disabled={loading}
            >
              {loading ? (
                <><span className="spinner" /> Signing in…</>
              ) : (
                <>Sign in <IconArrow /></>
              )}
            </button>

            <div className="login-footer">
              © {new Date().getFullYear()} Rafeeq · Egyptian Tourism Platform · All rights reserved
            </div>

          </div>
        </div>
    </div>
  );
}