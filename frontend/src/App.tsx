import { BrowserRouter } from 'react-router-dom';
import AppRoutes from './AppRoutes';

/** Root application component with BrowserRouter for production use. */
export default function App() {
  return (
    <BrowserRouter>
      <AppRoutes />
    </BrowserRouter>
  );
}
