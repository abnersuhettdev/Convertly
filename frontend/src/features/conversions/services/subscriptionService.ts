import { api } from "../../../lib/api";
import type { ApiResponse } from "../../../types/api";
import type { Subscription } from "../types/subscriptionTypes";

export async function getCurrentSubscription() {
  const response = await api.get<ApiResponse<Subscription>>("/subscription/me");
  return unwrapResponse(response.data);
}

function unwrapResponse<T>(response: ApiResponse<T>) {
  if (!response.success || response.data === null) {
    throw new Error(response.errors[0] ?? response.message);
  }

  return response.data;
}
