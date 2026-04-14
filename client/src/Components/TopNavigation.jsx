import { Menu } from 'lucide-react';
import { useSidebar } from '../contexts/SidebarContext';

const TopNavigation = () => {
  const { setIsMobileMenuOpen } = useSidebar();

  return (
    <header className="sticky top-0 z-30 bg-white border-b border-gray-200 px-4 py-3 mb-4">
      <button
        type="button"
        onClick={() => setIsMobileMenuOpen(true)}
        className="inline-flex items-center gap-2 rounded-md border border-gray-300 px-3 py-2 text-sm text-gray-700 hover:bg-gray-50 lg:hidden"
      >
        <Menu className="h-4 w-4" />
        Menu
      </button>
    </header>
  );
};

export default TopNavigation;
