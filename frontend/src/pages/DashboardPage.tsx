import { useAuth } from '../hooks/useAuth';

export function DashboardPage() {
  const { user, logout } = useAuth();

  return (
    <div className="min-h-screen bg-gray-900">
      <div className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          <div className="bg-bg-gray-800 shadow-lg rounded-lg p-6 border border-gray-700">
            <div className="flex justify-between items-center">
              <div>
                <h1 className="text-3xl font-bold text-white">Dashboard</h1>
                <p className="mt-2 text-sm text-gray-600">
                  Welcome back, <span className="font-medium">{user?.email}</span>
                </p>
              </div>
              <button
                onClick={logout}
                className="px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-red-600 hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500"
              >
                Logout
              </button>
            </div>
          </div>
        </div>

        <div className="px-4 py-6 sm:px-0">
          <div className="bg-bg-gray-800 shadow-lg rounded-lg p-6 border border-gray-700">
            <h2 className="text-xl font-semibold text-white mb-4">
              Protected Dashboard
            </h2>
            <p className="text-gray-400">
              You are now authenticated! This is a protected route.
            </p>
            <p className="text-gray-600 mt-2">
              The full dashboard with TCO calculations, charts, and vehicle data will be built next.
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
