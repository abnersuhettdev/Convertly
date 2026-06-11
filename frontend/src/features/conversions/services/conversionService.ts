import axios from "axios";
import type { TFunction } from "i18next";
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

export function getApiErrorMessage(error: unknown, t?: TFunction) {
  if (axios.isAxiosError<ApiResponse<unknown>>(error)) {
    if (!error.response) {
      return t?.("errors.network") ?? "Network error. Check whether the backend is running.";
    }

    const firstError = error.response.data?.errors?.[0];
    if (error.response.status === 422) {
      return mapApiErrorMessage(firstError, t) ?? t?.("errors.monthlyLimit") ?? "Monthly conversion limit reached.";
    }

    return mapApiErrorMessage(firstError, t)
      ?? mapApiErrorMessage(error.response.data?.message, t)
      ?? t?.("errors.requestFailed")
      ?? "Request failed.";
  }

  if (error instanceof Error) {
    return error.message;
  }

  return t?.("errors.requestFailed") ?? "Request failed.";
}

function mapApiErrorMessage(message: string | undefined, t?: TFunction) {
  if (!message || !t) {
    return message;
  }

  const translationKey = backendErrorMap[message];
  return translationKey ? t(translationKey) : message;
}

const backendErrorMap: Record<string, string> = {
  "Active subscription was not found": "errors.activeSubscriptionMissing",
  "Converted file has expired": "errors.downloadExpired",
  "Converted file is not available for download": "errors.downloadUnavailable",
  "Could not access this file. Check whether it is still available.": "errors.genericFileAccess",
  "File exceeds current plan size limit": "errors.fileTooLarge",
  "File extension is not supported": "errors.unsupportedExtension",
  "File MIME type is not supported": "errors.invalidMimeType",
  "File must not be empty": "errors.fileEmpty",
  "File type is not allowed": "errors.fileTypeBlocked",
  "Monthly conversion limit reached.": "errors.monthlyLimit",
  "Target format is not supported": "errors.targetUnsupported"
};

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
