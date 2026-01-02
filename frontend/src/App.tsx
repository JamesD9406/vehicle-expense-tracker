import { Routes, Route, Navigate } from 'react-router-dom';
import { ProtectedRoute } from './components/auth/ProtectedRoute';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { DashboardPage } from './pages/DashboardPage';

function App() {
  return (
    <div className="min-h-screen bg-gray-900">
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
        {/* Placeholder routes for future pages */}
        <Route
          path="/vehicles"
          element={
            <ProtectedRoute>
              <div className="min-h-screen bg-gray-900 flex items-center justify-center">
                <div className="text-center">
                  <h1 className="text-3xl font-bold text-white">Vehicles Page</h1>
                  <p className="text-gray-400 mt-2">Coming soon...</p>
                </div>
              </div>
            </ProtectedRoute>
          }
        />
        <Route
          path="/expenses"
          element={
            <ProtectedRoute>
              <div className="min-h-screen bg-gray-900 flex items-center justify-center">
                <div className="text-center">
                  <h1 className="text-3xl font-bold text-white">Expenses Page</h1>
                  <p className="text-gray-400 mt-2">Coming soon...</p>
                </div>
              </div>
            </ProtectedRoute>
          }
        />
        <Route
          path="/fuel"
          element={
            <ProtectedRoute>
              <div className="min-h-screen bg-gray-900 flex items-center justify-center">
                <div className="text-center">
                  <h1 className="text-3xl font-bold text-white">Fuel Page</h1>
                  <p className="text-gray-400 mt-2">Coming soon...</p>
                </div>
              </div>
            </ProtectedRoute>
          }
        />
        <Route
          path="/reports"
          element={
            <ProtectedRoute>
              <div className="min-h-screen bg-gray-900 flex items-center justify-center">
                <div className="text-center">
                  <h1 className="text-3xl font-bold text-white">Reports Page</h1>
                  <p className="text-gray-400 mt-2">Coming soon...</p>
                </div>
              </div>
            </ProtectedRoute>
          }
        />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </div>
  );
}

export default App;
