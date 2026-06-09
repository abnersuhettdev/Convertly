import { useQuery } from "@tanstack/react-query";
import { getConversionDetail } from "../services/conversionService";
import type { ConversionStatus } from "../types/conversionTypes";

const pollingStatuses: ConversionStatus[] = ["Pending", "Processing"];

export function useConversionDetail(conversionId: string | undefined) {
  return useQuery({
    queryKey: ["conversion", conversionId],
    queryFn: () => getConversionDetail(conversionId!),
    enabled: Boolean(conversionId),
    refetchInterval: (query) => {
      const status = query.state.data?.status;
      return status && pollingStatuses.includes(status) ? 3000 : false;
    },
  });
}
