import { Routes, Route, Navigate } from 'react-router-dom';
import { ProtectedRoute } from './components/auth/ProtectedRoute';

// Placeholder components for now
const LoginPage = () => <div className="p-8">Login Page - To be implemented</div>;
const RegisterPage = () => <div className="p-8">Register Page - To be implemented</div>;
const DashboardPage = () => <div className="p-8">Dashboard - Protected Route</div>;

function App() {
  return (
    <div className="min-h-screen bg-gray-100">
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route
          path="/"
          element={
            <ProtectedRoute>
              <DashboardPage />
            </ProtectedRoute>
          }
        />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </div>
  );
}

export default App;
