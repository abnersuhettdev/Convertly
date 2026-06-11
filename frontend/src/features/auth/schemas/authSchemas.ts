import { z } from "zod";
import type { TFunction } from "i18next";

export function getLoginSchema(t: TFunction) {
  return z.object({
    email: z.string().min(1, t("auth.validation.emailRequired")).email(t("auth.validation.emailInvalid")),
    password: z.string().min(1, t("auth.validation.passwordRequired")),
  });
}

export function getRegisterSchema(t: TFunction) {
  return z.object({
    name: z.string().min(1, t("auth.validation.nameRequired")),
    email: z.string().min(1, t("auth.validation.emailRequired")).email(t("auth.validation.emailInvalid")),
    password: z.string().min(8, t("auth.validation.passwordMin")),
  });
}

export const loginSchema = getLoginSchema(((key: string) => key) as TFunction);
export const registerSchema = getRegisterSchema(((key: string) => key) as TFunction);

export type LoginFormValues = z.infer<typeof loginSchema>;
export type RegisterFormValues = z.infer<typeof registerSchema>;
