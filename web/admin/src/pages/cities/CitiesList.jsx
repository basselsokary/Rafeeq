import React from 'react'
import Layout from '../../layouts/Layout'
import { Container, Button, Row, Col } from 'react-bootstrap'
import { Link } from 'react-router'

const CitiesList = () => {
    return (
        <>
            <Layout>
                <div style={{ paddingBottom: '50px', minHeight: '100vh', backgroundColor: '#fff8f0' }}>
                    <Container className='pt-4'>
                        <Row className='mt-2'>
                            <div className='d-flex justify-content-between'>
                                <div className='d-flex justify-content-center align-items-center gap-3'>
                                    <h1 className='fw-bold' style={{ color: "#251975" }}>Cities Management </h1>
                                    <span className='fw-bold rounded-4 px-2' style={{ color: "#251975", backgroundColor:'#c6b3f0' }}>27 Cities</span>
                                </div>
                                <Button as={Link} to='/cityEditor' className="rounded-3 px-4 py-2 fw-bold d-flex align-items-center gap-2"
                                    style={{ background: 'linear-gradient(45deg,#7C572D,#D4A574)', border: 'none' }}>
                                    <i className="bi bi-plus-lg"></i> Add New City
                                </Button>
                            </div>
                        </Row>
                        <Row className='mt-4 pt-3'>
                            
                        </Row>
                        <Row className='mt-4 pt-3'>
                            
                        </Row>
                    </Container>
                </div>
            </Layout>
        </>
    )
}

export default CitiesList