export type ConversionStatus = "Pending" | "Processing" | "Completed" | "Failed" | "Expired";

export type ConversionListItem = {
  id: string;
  sourceFileName: string;
  sourceFormat: string;
  targetFormat: string;
  status: ConversionStatus;
  createdAt: string;
  completedAt: string | null;
  downloadAvailable: boolean;
};

export type ConversionListResponse = {
  items: ConversionListItem[];
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
};

export type ConversionDetail = ConversionListItem & {
  errorMessage: string | null;
  startedAt: string | null;
  expiresAt: string | null;
};

export type CreateConversionResponse = {
  conversionId: string;
  status: ConversionStatus;
};

export type ConversionQuery = {
  page?: number;
  pageSize?: number;
  status?: ConversionStatus | "";
};
