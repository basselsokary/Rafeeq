const ACCESS_TOKEN_EXPIRES_AT_MS_KEY = 'rafeeq.accessTokenExpiresAtMs';

function safeNow() {
  return Date.now();
}

function toNumberOrNull(v) {
  const n = Number(v);
  return Number.isFinite(n) ? n : null;
}

export function getAccessTokenExpiresAtMs() {
  try {
    const raw = localStorage.getItem(ACCESS_TOKEN_EXPIRES_AT_MS_KEY);
    return toNumberOrNull(raw);
  } catch {
    return null;
  }
}

export function clearTokenExpirations() {
  try {
    localStorage.removeItem(ACCESS_TOKEN_EXPIRES_AT_MS_KEY);
  } catch {
    // ignore
  }
}

export function persistTokenExpirations(payload) {
  if (!payload || typeof payload !== 'object') return;

  const minutes = toNumberOrNull(payload.accessTokenExpirationInMinutes);
  if (minutes == null) return;

  const expiresAtMs = safeNow() + minutes * 60 * 1000;

  try {
    localStorage.setItem(ACCESS_TOKEN_EXPIRES_AT_MS_KEY, String(expiresAtMs));
  } catch {
    // ignore
  }
}

export function isAccessTokenExpired({ leewayMs = 30_000 } = {}) {
  const expiresAtMs = getAccessTokenExpiresAtMs();
  if (expiresAtMs == null) return true;

  return safeNow() >= (expiresAtMs - leewayMs);
}
