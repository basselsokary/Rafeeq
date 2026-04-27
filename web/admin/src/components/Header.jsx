import { useContext } from 'react'
import { Container, Nav, Navbar, Button } from 'react-bootstrap'
import { Link } from 'react-router-dom'
import { IoMoonOutline, IoSunnyOutline } from "react-icons/io5";
import '../styles/Header.css'
import logo from '../assets/logoo.png'
import { ThemeContext } from './Theme'

const links = {
    sites: '/sites',
    attractions: '/attractions',
    cities: '/cities',
    sponsors: '/sponsors',
}

const Header = () => {

    const { dark, toggleTheme } = useContext(ThemeContext);

    return (
        <header className='sticky-top'>
            <Navbar collapseOnSelect expand='lg' className={`w-100 ${dark ? 'dark-navbar-shadow' : ''}`}
                style={dark ? { backgroundColor: '#333' } : { backgroundColor: '#F5EFE7' }}>
                <Container>
                    <Navbar.Brand className='d-flex align-items-center'>
                        <Link to='/' className='logo'>
                            <img src={logo} alt="logo" className='img-fluid' />
                        </Link>
                    </Navbar.Brand>
                    <div className="d-flex align-items-center d-lg-none ms-auto">
                        <div onClick={toggleTheme} style={{ cursor: 'pointer', fontSize: '1.5rem' }} className="me-2 d-flex">
                            {dark ? <IoSunnyOutline className="text-warning" /> : <IoMoonOutline className="text-dark" />}
                        </div>
                        <Navbar.Toggle aria-controls='responsive-navbar-nav' className={dark ? 'dark-toggle' : ''} />
                    </div>
                    <Navbar.Collapse id='responsive-navbar-nav'>
                        <Nav className='ms-auto'>
                            {Object.entries(links).map(([key, link]) => (
                                <Nav.Link as={Link} to={link} key={key} className={`d-inline-flex justify-content-center text-capitalize ${dark ? 'text-warning' : 'text-dark'}`} >
                                    {key}
                                </Nav.Link>
                            ))}
                            <div className='d-flex align-items-center gap-3'>

                                <Nav className="button mx-auto mt-3 mt-lg-0">
                                    <Nav.Link as={Link} to="/login"
                                        className={`justify-content-center rounded-pill d-flex align-items-center gap-2 px-4 py-2
                                        ${dark ? 'bg-warning text-dark fw-bold' : 'bg-dark text-white'}`}
                                        style={{ transition: '0.3s', fontSize: '0.95rem' }}>
                                        Logout<i className="bi bi-box-arrow-right"></i>
                                    </Nav.Link>
                                </Nav>
                                <div
                                    onClick={toggleTheme}
                                    className="d-none d-lg-flex align-items-center justify-content-center"
                                    style={{ cursor: 'pointer', fontSize: '1.5rem', lineHeight: '0' }}>
                                    {dark ? <IoSunnyOutline className="text-warning" /> : <IoMoonOutline className="text-dark" />}
                                </div>
                            </div>
                        </Nav>
                    </Navbar.Collapse>
                </Container>
            </Navbar>
        </header>
    )
}

export default Header