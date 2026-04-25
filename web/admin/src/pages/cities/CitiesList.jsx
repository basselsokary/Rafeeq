import React, { useEffect, useState } from 'react'
import Layout from '../../layouts/Layout'
import { Container, Button, Row, Col, Form } from 'react-bootstrap'
import { Link } from 'react-router-dom'
import CityCard from '../../components/CityCard'
// import image from "../../assets/download.png"
import { getCities } from '../../api/citiesApi'

const CitiesList = () => {
    const [cities, setCities] = useState([]);

    useEffect(() => {
        const fetchCities = async () => {
            try {
                const response = await getCities();
                if (response && Array.isArray(response.data)) {
                    setCities(response.data);
                }
                else if (Array.isArray(response)) {
                    setCities(response);
                }
                else {
                    setCities([]);
                }
            } catch (error) {
                console.error("Error fetching cities:", error);
                setCities([]);
            }
        };
        fetchCities();
    }, []);

    return (
        <>
            <Layout>
                <div className='custom_body'>
                    <Container className='pt-4'>
                        <Row className='mt-2'>
                            <Col className='d-flex justify-content-between align-items-center flex-wrap gap-3'>
                                <div className='d-flex justify-content-center align-items-center gap-3'>
                                    <h1 className='fw-bold mb-0' style={{ color: "#251975", fontSize: '2.5rem' }}>Cities Management </h1>
                                    <span className='fw-bold rounded-pill px-3 py-1' style={{ color: "#251975", backgroundColor: '#dcd0ff', fontsize: '0.9rem', display: 'inline-block' }}>27 Cities</span>
                                </div>
                                <Button as={Link} to='/cityEditor' className="rounded-3 px-4 py-2 fw-bold d-flex align-items-center gap-2 shadow-sm"
                                    style={{ background: 'linear-gradient(45deg,#7C572D,#D4A574)', border: 'none', height: 'fit-content' }}>
                                    <i className="bi bi-plus-lg"></i> Add New City
                                </Button>
                            </Col>
                        </Row>
                        <Row className='mt-4 p-3 justify-content-between align-items-center'
                            style={{ backgroundColor: '#F5EFE7', borderRadius: '10px', boxShadow: '0 4px 8px rgba(0,0,0,0.1)' }}>
                            <Col xl={5} lg={5} md={7} sm={12} xs={12} >
                                <Form className='d-flex'>
                                    <div className='position-relative w-100'>
                                        <Form.Control type='search'
                                            placeholder='Search by city name...'
                                            className='rounded-3 pe-5 border-0 shadow-sm' 
                                            style={{ height: '45px' }}/>
                                        <Button
                                            type="submit"
                                            variant="link"
                                            className="position-absolute top-50 end-0 translate-middle-y p-0 me-3 border-0 shadow-none">
                                            <i className="bi bi-search text-secondary fs-5"></i>
                                        </Button>
                                    </div>
                                </Form>
                            </Col>
                            <Col xl={4} lg={4} md={5} sm={12} xs={12}
                                className='d-flex justify-content-md-end align-items-center mt-3 mt-md-0'>
                                <div className='d-flex align-items-center gap-2'>
                                    <span className='text-secondary' style={{whiteSpace:'nowrap'}}>Sort by:</span>
                                    <Form.Select className='border-3 rounded-3 shadow-sm' 
                                    style={{color:'#444',height:'45px',minWidth:'160px'}}>
                                        <option value="order">Display order</option>
                                        <option value="sites">Total sites(descending)</option>
                                    </Form.Select>
                                </div>
                            </Col>
                        </Row>
                        <Row className='mt-4 pt-3'>
                            {cities.length === 0 ? (
                                <Col className='d-flex flex-column justify-content-center align-items-center gap-3 py-5 w-100'>
                                    <i className="bi bi-search text-secondary" style={{ fontSize: '3rem' }}></i>
                                    <h3 className='fw-bold text-muted'>No cities found</h3>
                                </Col>
                            ) : (
                                cities.map(city => (
                                    <Col key={city.id} xl={3} lg={4} md={6} sm={6} xs={12} className='mb-4'>
                                        <CityCard
                                            id={city.id}
                                            image={city.imageUrl}
                                            location={{
                                                lat: city.centerLocation?.latitude ,
                                                lng: city.centerLocation?.longitude 
                                            }}
                                            name={city.name}
                                            sitesNum={city.totalSites}
                                        />
                                    </Col>
                                ))
                            )}
                        </Row>
                    </Container>
                </div>
            </Layout>
        </>
    )
}

export default CitiesList