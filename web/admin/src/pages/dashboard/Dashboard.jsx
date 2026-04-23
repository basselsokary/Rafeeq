import React from 'react'
import Layout from '../../layouts/Layout'
import { Card, Container, Row, Col } from 'react-bootstrap'
import { MapContainer, TileLayer, Marker, Popup, GeoJSON } from 'react-leaflet'
import egyptGeoJson from '../../assets/egypt.json'
import CustomCard from '../../components/CustomCard'

const cardsData = {
  "Total Users": {
    value: 999,
    icon: "bi bi-people"
  },
  "Active Users": {
    value: 400,
    icon: "bi bi-person-check"
  },
  "Total Sites": {
    value: 120,
    icon: "bi bi-geo-alt"
  },
  "Total Cities": {
    value: 20,
    icon: "bi bi-buildings"
  }
};

const EgyptPosition = [26.8206, 30.8025];

const Governorates = [
  { id: 1, name: "Cairo", position: [30.0444, 31.2357], sites: 150 },
  { id: 2, name: "Alexandria", position: [31.2001, 29.9187], sites: 85 },
  { id: 3, name: "Luxor", position: [25.6872, 32.6396], sites: 200 },
  { id: 4, name: "Aswan", position: [24.0889, 32.8998], sites: 120 },
  { id: 5, name: "Giza", position: [30.0131, 31.2089], sites: 95 },
];

const topSites = [
  { name: 'Pyramids', rating: 4.9 },
  { name: 'Grand Egyptian Museum', rating: 4.5 },
  { name: 'Cairo Tower', rating: 4.0 },
  { name: 'Luxor Temple', rating: 3.0 },
  { name: 'Karnak Temple', rating: 3.5 },
  { name: 'Karnak Temple', rating: 3.5 },
  { name: 'Karnak Temple', rating: 3.5 },
  { name: 'Karnak Temple', rating: 3.5 },
]

const Dashboard = () => {
  return (
    <>
      <Layout>
        <div style={{ paddingBottom: '50px', minHeight: '100vh', backgroundColor: '#fff8f0' }}>
          <Container className='pt-4'>
            <Row className='g-4 mt-2'>
              {Object.keys(cardsData).map((key) => (
                <Col key={key} xl={3} lg={3} md={6} sm={6} xs={6}>
                  <CustomCard title={key} value={cardsData[key].value} icon={cardsData[key].icon} />
                </Col>
              ))}
            </Row>
            <Row className='g-4 mt-4 pt-3'>
              <Col xl={6} lg={6} md={12} sm={12} xs={12}>
                <MapContainer center={EgyptPosition} zoom={6}
                  zoomControl={false}
                  style={{ height: '500px', width: '100%', backgroundColor: 'transparent' }}>
                  <TileLayer
                    url="https://{s}.basemaps.cartocdn.com/rastertiles/voyager_nolabels/{z}/{x}/{y}{r}.png"
                  />
                  <GeoJSON data={egyptGeoJson} style={{
                    fillColor: "#eab308",
                    weight: 2,
                    opacity: 1,
                    color: "#ca8a04",
                    fillOpacity: 1
                  }} />
                  {Governorates.map((gov) => (
                    <Marker key={gov.id} position={gov.position}  >
                      <Popup>
                        <div style={{ textAlign: 'center' }}>
                          <strong style={{ color: '#7C572D' }}>{gov.name}</strong> <br />
                          Sites : {gov.sites}
                        </div>
                      </Popup>
                    </Marker>
                  ))}
                </MapContainer>
              </Col>
              <Col xl={6} lg={6} md={12} sm={12} xs={12}>
                <Card className='border-0 rounded-4 shadow-sm p-3'>
                  <h6 className='fw-bold mb-4'>Top 10 Rated Sites</h6>
                  {topSites.map((site, index) => (
                    <div key={index} className='mb-4'>
                      <div className='d-flex justify-content-between mb-2'>
                        <small className='fw-bold' style={{ color: '#555' }}>{site.name}</small>
                        <small className='fw-bold'>{site.rating}</small>
                      </div>
                      <div className='progress' style={{ height: '5px', backgroundColor: '#f0e5d8' }}>
                        <div className='progress-bar' style={{
                          width: `${(site.rating / 5) * 100}%`,
                          backgroundColor: '#8b6b4a',
                          borderRadius: '5px'
                        }}></div>
                      </div>
                    </div>
                  ))}
                </Card>
              </Col>
            </Row>
          </Container>
        </div>
      </Layout >
    </>
  )
}

export default Dashboard