import axios from "axios";
import { api } from "../../../lib/api";
import type { ApiResponse } from "../../../types/api";
import type {
  ConversionDetail,
  ConversionListResponse,
  ConversionQuery,
  CreateConversionResponse,
} from "../types/conversionTypes";

export async function getConversions(query: ConversionQuery = {}) {
  const response = await api.get<ApiResponse<ConversionListResponse>>("/conversions", {
    params: {
      page: query.page ?? 1,
      pageSize: query.pageSize ?? 10,
      status: query.status || undefined,
    },
  });

  return unwrapResponse(response.data);
}

export async function getConversionDetail(conversionId: string) {
  const response = await api.get<ApiResponse<ConversionDetail>>(`/conversions/${conversionId}`);
  return unwrapResponse(response.data);
}

export async function createConversion(file: File) {
  const formData = new FormData();
  formData.append("file", file);
  formData.append("targetFormat", "pdf");

  const response = await api.post<ApiResponse<CreateConversionResponse>>("/conversions", formData, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });

  return unwrapResponse(response.data);
}

export async function downloadConversion(conversionId: string) {
  const response = await api.get<Blob>(`/conversions/${conversionId}/download`, {
    responseType: "blob",
  });

  return {
    blob: response.data,
    fileName: getFileName(response.headers["content-disposition"]),
  };
}

export function getApiErrorMessage(error: unknown) {
  if (axios.isAxiosError<ApiResponse<unknown>>(error)) {
    if (!error.response) {
      return "Network error. Check whether the backend is running.";
    }

    const firstError = error.response.data?.errors?.[0];
    if (error.response.status === 422) {
      return firstError ?? "Monthly conversion limit reached.";
    }

    return firstError ?? error.response.data?.message ?? "Request failed.";
  }

  if (error instanceof Error) {
    return error.message;
  }

  return "Request failed.";
}

function unwrapResponse<T>(response: ApiResponse<T>) {
  if (!response.success || response.data === null) {
    throw new Error(response.errors[0] ?? response.message);
  }

  return response.data;
}

function getFileName(contentDisposition: string | undefined) {
  if (!contentDisposition) {
    return "converted.pdf";
  }

  const fileNameMatch = contentDisposition.match(/filename\*?=(?:UTF-8''|")?([^";]+)/i);
  if (!fileNameMatch?.[1]) {
    return "converted.pdf";
  }

  return decodeURIComponent(fileNameMatch[1].replaceAll('"', ""));
}
