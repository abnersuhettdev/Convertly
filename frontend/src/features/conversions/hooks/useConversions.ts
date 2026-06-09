import { useQuery } from "@tanstack/react-query";
import { getConversions } from "../services/conversionService";
import type { ConversionQuery } from "../types/conversionTypes";

export function useConversions(query: ConversionQuery = {}) {
  return useQuery({
    queryKey: ["conversions", query],
    queryFn: () => getConversions(query),
  });
}
