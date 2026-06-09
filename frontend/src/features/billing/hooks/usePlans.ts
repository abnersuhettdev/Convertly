import { useQuery } from "@tanstack/react-query";
import { getPlans } from "../services/billingService";

export function usePlans() {
  return useQuery({
    queryKey: ["plans"],
    queryFn: getPlans,
  });
}
