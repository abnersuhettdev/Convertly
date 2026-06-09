import { useMutation, useQueryClient } from "@tanstack/react-query";
import { createConversion } from "../services/conversionService";

export function useCreateConversion() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: createConversion,
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["conversions"] });
      void queryClient.invalidateQueries({ queryKey: ["subscription", "me"] });
    },
  });
}
