import axios from 'axios';
import type { TFunction } from 'i18next';
import { api } from '../../../lib/api';
import type { ApiResponse } from '../../../types/api';
import type {
  AuthResponse,
  LoginRequest,
  RegisterRequest,
  User,
} from '../types/authTypes';

export async function login(request: LoginRequest) {
  const response = await api.post<ApiResponse<AuthResponse>>(
    '/auth/login',
    request,
  );
  return unwrapResponse(response.data);
}

export async function register(request: RegisterRequest) {
  const response = await api.post<ApiResponse<AuthResponse>>(
    '/auth/register',
    request,
  );
  return unwrapResponse(response.data);
}

export async function getMe() {
  const response = await api.get<ApiResponse<User>>('/auth/me');
  return unwrapResponse(response.data);
}

export function getAuthErrorMessage(error: unknown, t?: TFunction) {
  if (axios.isAxiosError<ApiResponse<unknown>>(error)) {
    const body = error.response?.data;
    const firstError = body?.errors?.[0];
    return firstError ?? body?.message ?? t?.('errors.authFailed') ?? 'Authentication request failed';
  }

  if (error instanceof Error) {
    return error.message;
  }

  return t?.('errors.authFailed') ?? 'Authentication request failed';
}

function unwrapResponse<T>(response: ApiResponse<T>) {
  if (!response.success || response.data === null) {
    throw new Error(response.errors[0] ?? response.message);
  }

  return response.data;
}
