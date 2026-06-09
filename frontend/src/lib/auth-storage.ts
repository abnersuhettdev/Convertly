const accessTokenKey = "convertly.accessToken";
const refreshTokenKey = "convertly.refreshToken";

function canUseStorage() {
  return typeof window !== "undefined" && typeof window.localStorage !== "undefined";
}

export function saveAccessToken(token: string) {
  if (canUseStorage()) {
    window.localStorage.setItem(accessTokenKey, token);
  }
}

export function saveRefreshToken(token: string) {
  if (canUseStorage()) {
    window.localStorage.setItem(refreshTokenKey, token);
  }
}

export function getAccessToken() {
  return canUseStorage() ? window.localStorage.getItem(accessTokenKey) : null;
}

export function getRefreshToken() {
  return canUseStorage() ? window.localStorage.getItem(refreshTokenKey) : null;
}

export function clearTokens() {
  if (canUseStorage()) {
    window.localStorage.removeItem(accessTokenKey);
    window.localStorage.removeItem(refreshTokenKey);
  }
}
