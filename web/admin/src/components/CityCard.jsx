import { Card,Button } from "react-bootstrap";

function CityCard({ image, location, name, sitesNum }) {
    return (
        <Card className="rounded-4 shadow-sm overflow-hidden">
            <div className="position-relative">
                {image ? (
                    <Card.Img variant="top" src={image} style={{ height: '200px', objectFit: 'cover' }} />
                ) : (
                    <div
                        className="d-flex justify-content-center align-items-center bg-light"
                        style={{ height: '200px' }}
                    >
                        <i className="bi bi-image text-secondary" style={{ fontSize: '3rem' }}></i>
                    </div>
                )}
                {/* <Card.Img variant="top" src={image} style={{ height: '200px', objectFit: 'cover' }} /> */}
                <div className="position-absolute bottom-0 start-0 p-3 w-100 text-white"
                    style={{ background: 'linear-gradient(to top, rgba(0,0,0,0.6), transparent)' }}>
                    <span className="m-0"><i className="bi bi-geo-alt-fill" /> {location.lat.toFixed(4)}, {location.lng.toFixed(4)}</span>
                </div>
            </div>
            <Card.Body>
                <div className="d-flex justify-content-between align-items-center ">
                    <h3 className="fw-bold" style={{ color: '#251975' }}>{name}</h3>
                    <p className="fw-bold" style={{ color: '#251975' }}><i className="bi bi-buildings" /> {sitesNum} Sites</p>
                </div>
                <div className="d-flex gap-2 mt-4">
                    <Button
                        className="w-100 border-0 fw-bold py-2 d-flex align-items-center justify-content-center"
                        style={{
                            backgroundColor: '#fff8f0',
                            color: '#251975',
                            borderRadius: '12px',
                            height: '55px'
                        }}>
                        View Sites
                    </Button>
                    <Button
                        variant="light"
                        className="border-0 d-flex align-items-center justify-content-center"
                        style={{
                            backgroundColor: '#fff8f0',
                            borderRadius: '12px',
                            width: '65px',
                            height: '55px'
                        }}>
                        <i className="bi bi-pencil-fill" style={{ color: '#251975' }}></i>
                    </Button>
                </div>
            </Card.Body>
        </Card>
    )
}

export default CityCard;