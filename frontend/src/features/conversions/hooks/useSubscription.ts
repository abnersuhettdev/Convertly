import { useQuery } from "@tanstack/react-query";
import { getCurrentSubscription } from "../services/subscriptionService";

export function useSubscription() {
  return useQuery({
    queryKey: ["subscription", "me"],
    queryFn: getCurrentSubscription,
  });
}
