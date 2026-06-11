import { Route, Routes } from "react-router-dom";
import { AppLayout } from "../components/layout/AppLayout";
import { AccountPage } from "../features/account/AccountPage";
import { LoginPage } from "../features/auth/pages/LoginPage";
import { RegisterPage } from "../features/auth/pages/RegisterPage";
import { BillingPage } from "../features/billing/pages/BillingPage";
import { ConversionDetailPage } from "../features/conversions/pages/ConversionDetailPage";
import { ConversionsPage } from "../features/conversions/pages/ConversionsPage";
import { NewConversionPage } from "../features/conversions/pages/NewConversionPage";
import { DashboardPage } from "../features/dashboard/pages/DashboardPage";
import { CopyrightPage } from "./CopyrightPage";
import { LandingPage } from "./LandingPage";
import { ProtectedRoute } from "./ProtectedRoute";
import { TermsPage } from "./TermsPage";

export function AppRoutes() {
  return (
    <Routes>
      <Route element={<AppLayout />}>
        <Route index element={<LandingPage />} />
        <Route path="terms" element={<TermsPage />} />
        <Route path="copyright" element={<CopyrightPage />} />
        <Route path="login" element={<LoginPage />} />
        <Route path="register" element={<RegisterPage />} />
        <Route element={<ProtectedRoute />}>
          <Route path="dashboard" element={<DashboardPage />} />
          <Route path="conversions" element={<ConversionsPage />} />
          <Route path="conversions/new" element={<NewConversionPage />} />
          <Route path="conversions/:id" element={<ConversionDetailPage />} />
          <Route path="billing" element={<BillingPage />} />
          <Route path="account" element={<AccountPage />} />
        </Route>
      </Route>
    </Routes>
  );
}
