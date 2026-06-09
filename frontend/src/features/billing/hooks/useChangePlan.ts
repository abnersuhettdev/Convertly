import { useMutation, useQueryClient } from "@tanstack/react-query";
import { changePlan } from "../services/billingService";
import type { Plan } from "../../conversions/types/subscriptionTypes";

export function useChangePlan() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (planSlug: Plan["slug"]) => changePlan(planSlug),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["subscription"] });
      void queryClient.invalidateQueries({ queryKey: ["conversions"] });
      void queryClient.invalidateQueries({ queryKey: ["plans"] });
    },
  });
}
