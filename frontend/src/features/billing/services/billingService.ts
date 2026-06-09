import { api } from "../../../lib/api";
import type { ApiResponse } from "../../../types/api";
import type { Plan, Subscription } from "../../conversions/types/subscriptionTypes";

export async function getPlans() {
  const response = await api.get<ApiResponse<Plan[]>>("/plans");
  return unwrapResponse(response.data);
}

export async function changePlan(planSlug: Plan["slug"]) {
  const response = await api.post<ApiResponse<Subscription>>("/subscription/change-plan", {
    planSlug,
  });
  return unwrapResponse(response.data);
}

function unwrapResponse<T>(response: ApiResponse<T>) {
  if (!response.success || response.data === null) {
    throw new Error(response.errors[0] ?? response.message);
  }

  return response.data;
}
