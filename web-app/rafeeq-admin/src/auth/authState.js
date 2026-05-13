let state = {
  isAuthenticated: false,
  hasChecked: false,
};

const listeners = new Set();

export function getAuthState() {
  return state;
}

export function setAuthState(patch) {
  state = { ...state, ...patch };
  listeners.forEach((l) => l(state));
}

export function subscribeAuth(listener) {
  listeners.add(listener);
  return () => listeners.delete(listener);
}
