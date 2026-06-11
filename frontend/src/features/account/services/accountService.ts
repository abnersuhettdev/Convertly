import axios from "axios";
import type { TFunction } from "i18next";
import { api } from "../../../lib/api";
import type { ApiResponse } from "../../../types/api";

export type ChangePasswordRequest = {
  currentPassword: string;
  newPassword: string;
};

export type DeleteAccountRequest = {
  currentPassword: string;
};

export async function changePassword(request: ChangePasswordRequest) {
  const response = await api.patch<ApiResponse<object>>("/account/password", request);
  return unwrapResponse(response.data);
}

export async function deleteAccount(request: DeleteAccountRequest) {
  const response = await api.delete<ApiResponse<object>>("/account", {
    data: request,
  });
  return unwrapResponse(response.data);
}

export function getAccountErrorMessage(error: unknown, t: TFunction) {
  if (axios.isAxiosError<ApiResponse<unknown>>(error)) {
    if (!error.response) {
      return t("errors.network");
    }

    const firstError = error.response.data?.errors?.[0];
    return mapAccountError(firstError, t)
      ?? mapAccountError(error.response.data?.message, t)
      ?? t("errors.requestFailed");
  }

  if (error instanceof Error) {
    return error.message;
  }

  return t("errors.requestFailed");
}

function mapAccountError(message: string | undefined, t: TFunction) {
  if (!message) {
    return undefined;
  }

  const translationKey = accountErrorMap[message];
  return translationKey ? t(translationKey) : message;
}

const accountErrorMap: Record<string, string> = {
  "Authenticated user was not found": "errors.authFailed",
  "Current password is invalid": "account.messages.currentPasswordInvalid",
  "Current password is required": "account.validation.currentPasswordRequired",
  "New password is required": "account.validation.newPasswordRequired",
  "New password must be at least 8 characters": "account.validation.newPasswordMin",
  "New password must be different from current password": "account.validation.newPasswordDifferent",
};

function unwrapResponse<T>(response: ApiResponse<T>) {
  if (!response.success || response.data === null) {
    throw new Error(response.errors[0] ?? response.message);
  }

  return response.data;
}
