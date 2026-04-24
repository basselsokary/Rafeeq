import {useContext} from 'react'
import { Container, Nav, Navbar,Button } from 'react-bootstrap'
import { Link } from 'react-router-dom'
import '../styles/Header.css'
import logo from '../assets/logoo.png'
import { ThemeContext } from './Theme'

const links = {
    sites : '/sites',
    attractions : '/attractions',
    cities : '/cities',
    sponsors : '/sponsors',
}

const Header = () => {

    const {dark,toggleTheme} = useContext(ThemeContext);

    return (
        <header className='sticky-top'>
            <Navbar collapseOnSelect expand='lg' className='w-100' style= {dark ? { backgroundColor: '#333' } : { backgroundColor: '#F5EFE7' }}>
                <Container>
                    <Navbar.Brand className='d-flex align-items-center'>
                        <Link to='/' className='logo'>
                            <img src={logo} alt="logo" className='img-fluid' />
                        </Link>
                    {/* <Button onClick={toggleTheme}>
                        {dark ? "☀️" : "🌙"}
                    </Button> */}
                    </Navbar.Brand>
                    <Navbar.Toggle aria-controls='responsive-navbar-nav'/>
                    <Navbar.Collapse id='responsive-navbar-nav'>
                        <Nav className='ms-auto'>
                            {Object.entries(links).map(([key, link]) => (
                                <Nav.Link as={Link} to={link} key={key} className="d-inline-flex justify-content-center text-dark text-capitalize" >
                                    {key}
                                </Nav.Link>
                            ))}
                            <Nav className="button mx-auto mt-3 mt-lg-0">
                                <Nav.Link as={Link} to="/login" className='justify-content-center bg-dark text-white rounded-pill d-flex align-items-center gap-2 px-4 py-1'>
                                    Logout<i className="bi bi-box-arrow-right"></i>
                                </Nav.Link>
                            </Nav>
                        </Nav>
                    </Navbar.Collapse>
                </Container>
            </Navbar>
        </header>
    )
}

export default Header