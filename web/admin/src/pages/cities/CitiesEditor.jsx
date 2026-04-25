import React, { useEffect, useState } from 'react'
import { useForm } from "react-hook-form";
import Layout from '../../layouts/Layout'
import { Container, Row, Button, Col, Form, ListGroup } from 'react-bootstrap'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { MapContainer, TileLayer, Marker, Popup, useMap } from 'react-leaflet'
import CustomCard from '../../components/CustomCard'
import { useQuill } from 'react-quilljs';
import { addCity, updateCity, deleteCity, getCity, updateLocalizedContent } from '../../api/citiesApi';

function ChangeView({ center }) {
    const map = useMap();
    useEffect(() => {
        if (!isNaN(center[0]) && !isNaN(center[1])) {
            map.flyTo(center, map.getZoom());
        }
    }, [center, map]);
    return null;
}

const languages = [
    { code: "en", name: "english" },
    { code: "ar", name: "arabic" },
    { code: "fr", name: "french" },
    { code: "de", name: "german" },
];

const CitiesEditor = () => {

    const { id: cityId } = useParams();
    const navigate = useNavigate();
    const isEdit = Boolean(cityId);
    const [selectedLang, setSelectedLang] = useState("en");
    const [posterPreview, setPosterPreview] = useState(null);
    const [cityNameTitle, setCityNameTitle] = useState("");

    const { quill, quillRef } = useQuill({
        modules: {
            toolbar: [
                ['bold', 'italic'],
                [{ 'list': 'bullet' }],
                ['link'],
                ['clean']
            ],
        },
        placeholder: 'Write city description...',
    });

    const { register, handleSubmit, setValue, getValues, watch, reset } = useForm({
        defaultValues: {
            DisplayOrder: 1,
            centerLocation: {
                latitude: 30.0444,
                longitude: 31.2357,
            },
            image: null,
            LocalizedContents: languages.map(lang => ({
                language: lang.name.toLowerCase(),
                name: "",
                description: ""
            }))
        }
    });

    const [watchLat, watchLng] = watch(["centerLocation.latitude", "centerLocation.longitude"]);
    const position = [
        parseFloat(watchLat) || 30.0444,
        parseFloat(watchLng) || 31.2357
    ];
    const handleLangChange = (newLangCode) => {
        const currentIndex = languages.findIndex(l => l.code === selectedLang);
        if (quill) {
            setValue(`LocalizedContents.${currentIndex}.description`, quill.root.innerHTML);
        }
        setSelectedLang(newLangCode);
        const nextIndex = languages.findIndex(l => l.code === newLangCode);
        const savedData = getValues(`LocalizedContents.${nextIndex}.description`) || "";
        if (quill) {
            quill.root.innerHTML = savedData;
        }
    };

    useEffect(() => {
        if (isEdit) {
            const fetchCityData = async () => {
                try {
                    const cityData = await getCity(cityId);
                    const enTrans = cityData.localizedContents?.find(t => t.language === "english");
                    setCityNameTitle(enTrans?.name || "");
                    reset({
                        DisplayOrder: cityData.displayOrder,
                        centerLocation: {
                            latitude: cityData.centerLocation.latitude,
                            longitude: cityData.centerLocation.longitude,
                        },
                        LocalizedContents: languages.map(lang => {
                            const existingTrans = cityData.localizedContents?.find(
                                t => t.language.toLowerCase() === lang.name.toLowerCase()
                            );
                            return {
                                contentId: existingTrans?.contentId || null,
                                language: lang.name.toLowerCase(),
                                name: existingTrans?.name || "",
                                description: existingTrans?.description || ""
                            };
                        })
                    });
                    if (cityData.imageUrl) {
                        setPosterPreview(cityData.imageUrl);
                    }
                } catch (error) {
                    console.error("Error fetching city data:", error);
                }
            };
            fetchCityData();
        }
    }, [cityId, isEdit, reset]);

    useEffect(() => {
        if (quill) {
            const currentIndex = languages.findIndex(l => l.code === selectedLang);
            const currentDescription = getValues(`LocalizedContents.${currentIndex}.description`);
            const timer = setTimeout(() => {
                if (currentDescription) {
                    quill.clipboard.dangerouslyPasteHTML(currentDescription);
                } else {
                    quill.root.innerHTML = "";
                }
            }, 100);
            return () => clearTimeout(timer);
        }
    }, [quill, selectedLang, watch(`LocalizedContents`)]);


    useEffect(() => {
        if (quill) {
            const handleTextChange = () => {
                const currentIndex = languages.findIndex(l => l.code === selectedLang);
                setValue(`LocalizedContents.${currentIndex}.description`, quill.root.innerHTML);
            };
            quill.on('text-change', handleTextChange);
            return () => {
                quill.off('text-change', handleTextChange);
            };
        }
    }, [quill, selectedLang, setValue]);

    const handleImage = (e) => {
        const file = e.target.files[0];
        if (file) {
            setValue("image", file);
            setPosterPreview(URL.createObjectURL(file));
        }
    };

    const handleDeleteCity = async (cityId) => {
        try {
            await deleteCity(cityId);
            navigate('/cities');
        }
        catch (error) {
            console.error("Error deleting city:", error);
        }
    };

    const onSubmit = async (data) => {
        try {
            const formData = new FormData();
            formData.append("DisplayOrder", data.DisplayOrder);
            formData.append("CenterLocation.Latitude", data.centerLocation.latitude);
            formData.append("CenterLocation.Longitude", data.centerLocation.longitude);
            if (data.image) {
                formData.append("Image", data.image);
            }
            data.LocalizedContents?.forEach((trans, index) => {
                if (trans.contentId) {
                    formData.append(`LocalizedContents[${index}].ContentId`, trans.contentId);
                }
                formData.append(`LocalizedContents[${index}].Language`, trans.language);
                formData.append(`LocalizedContents[${index}].Name`, trans.name);
                formData.append(`LocalizedContents[${index}].Description`, trans.description);
            });
            if (isEdit) {
                const coreFormData = new FormData();
                coreFormData.append("DisplayOrder", data.DisplayOrder);
                coreFormData.append("CenterLocation.Latitude", data.centerLocation.latitude);
                coreFormData.append("CenterLocation.Longitude", data.centerLocation.longitude);
                if (data.image instanceof File) {
                    coreFormData.append("Image", data.image);
                }
                await updateCity(cityId, coreFormData);
                const translationsPayload = data.LocalizedContents.map(t => ({
                    contentId: t.contentId, 
                    name: t.name,
                    description: t.description
                }));
                await updateLocalizedContent(cityId, translationsPayload);
                console.log("Everything Updated in 2 Requests!");
                navigate('/cities');
            } else {
                await addCity(formData);
                navigate('/cities');
            }
        } catch (error) {
            console.error("Error:", error);
        }
    };
    return (
        <>
            <Layout>
                <div className='custom_body'>
                    <Container className='pt-4'>
                        <Row className='mt-2'>
                            <Col className='d-flex justify-content-between align-items-center flex-wrap gap-3'>
                                <h1 className='fw-bold mb-0' style={{ color: "#7C572D", fontSize: '2.5rem' }}>
                                    {isEdit ? `Edit City: ${cityNameTitle}` : "Add New City"}
                                </h1>
                                <div className='gap-3 d-flex justify-content-center align-items-center'>
                                    {isEdit && (
                                        <Button
                                            variant="danger rounded-3 px-4 py-2 fw-bold shadow-sm"
                                            onClick={() => handleDeleteCity(cityId)}>
                                            Delete City
                                        </Button>
                                    )}
                                    <Button type='button'
                                        form="city-form"
                                        onClick={() => handleSubmit(onSubmit)()}
                                        className="rounded-3 px-4 py-2 fw-bold shadow-sm"
                                        style={{ background: 'linear-gradient(45deg,#7C572D,#D4A574)', border: 'none', height: 'fit-content' }}>
                                        {isEdit ? "Update City" : "Add City"}
                                    </Button>
                                </div>
                            </Col>
                        </Row>
                        <Form id="city-form" onSubmit={handleSubmit(onSubmit)}>
                            <div className='mt-4 p-4 rounded-3 shadow' style={{ backgroundColor: '#F5EFE7' }}>
                                <h6 className="fw-bold mb-4" style={{ color: "#7C572D" }}>1. Basic Information</h6>
                                <Row>
                                    <Col md={3} sm={3} xs={3} className="border-end bg-light bg-opacity-25" style={{ minHeight: '400px' }}>
                                        <ListGroup variant="flush">
                                            {languages.map((lang) => (
                                                <ListGroup.Item
                                                    key={lang.code}
                                                    active={selectedLang === lang.code}
                                                    onClick={() => handleLangChange(lang.code)}
                                                    className={`py-3 px-4 border-0 d-flex align-items-center gap-3 text-capitalize ${selectedLang === lang.code ? 'bg-white' : 'bg-transparent'}`}
                                                    style={{
                                                        cursor: "pointer",
                                                        color: selectedLang === lang.code ? "#7C572D" : "#A0A0A0",
                                                        fontWeight: selectedLang === lang.code ? "600" : "400",
                                                        borderLeft: selectedLang === lang.code ? "4px solid #7C572D" : "4px solid transparent"
                                                    }}>
                                                    {lang.name}
                                                </ListGroup.Item>
                                            ))}
                                        </ListGroup>
                                    </Col>
                                    <Col md={9} sm={9} xs={9} className="p-4 bg-white">
                                        {languages.map((lang, index) => (
                                            <div key={lang.code} style={{ display: selectedLang === lang.code ? 'block' : 'none' }}>
                                                <Form.Group className="mb-4">
                                                    <label className="text-muted tiny fw-bold mb-2">
                                                        CITY NAME ({lang.code.toUpperCase()})
                                                    </label>
                                                    <Form.Control
                                                        {...register(`LocalizedContents.${index}.name`)}
                                                        className="border-0 py-3 rounded-3"
                                                        style={{ backgroundColor: '#FDFBF7', color: '#7C572D' }}
                                                        placeholder={`Enter city name in ${lang.name}...`} />
                                                </Form.Group>
                                                <input type="hidden" {...register(`LocalizedContents.${index}.language`)} />
                                            </div>
                                        ))}
                                        <Form.Group>
                                            <label className="text-muted tiny fw-bold mb-2">
                                                DESCRIPTION ({selectedLang.toUpperCase()})
                                            </label>
                                            <div className="rounded-3 overflow-hidden" style={{ border: '1px solid #FDFBF7' }}>
                                                <div ref={quillRef} style={{ height: '250px', backgroundColor: '#FDFBF7' }} />
                                            </div>
                                        </Form.Group>
                                    </Col>
                                </Row>
                            </div>
                            <div className='mt-4 p-4 rounded-3 shadow' style={{ backgroundColor: '#F5EFE7' }}>
                                <h6 className="fw-bold mb-4" style={{ color: "#7C572D" }}>2. Geographic Configuration</h6>
                                <Row>
                                    <Col xl={7} lg={7} >
                                        <MapContainer
                                            zoomControl={false}
                                            scrollWheelZoom={false}
                                            dragging={false}
                                            center={position} zoom={9} style={{ height: '400px', borderRadius: '12px' }}>
                                            <TileLayer
                                                url="https://mt1.google.com/vt/lyrs=m&x={x}&y={y}&z={z}"
                                            />
                                            <ChangeView center={position} />
                                            <Marker position={position}>
                                            </Marker>
                                        </MapContainer>
                                    </Col>
                                    <Col xl={5} lg={5} >
                                        <div className='p-3'>
                                            <h6 className="text-muted mb-3" style={{ fontSize: '0.9rem' }}>CENTER COORDINATES</h6>
                                            <Row>
                                                <Col xs={6}>
                                                    <Form.Group className="mb-3">
                                                        <div className="d-flex align-items-center bg-white border rounded-3 px-3 shadow-sm">
                                                            <Form.Control
                                                                type="number"
                                                                step="any"
                                                                {...register("centerLocation.latitude")}
                                                                className="border-0 shadow-none px-0"
                                                            />
                                                            <span className="text-muted small ms-2 fw-bold">LAT</span>
                                                        </div>
                                                    </Form.Group>
                                                </Col>
                                                <Col xs={6}>
                                                    <Form.Group className="mb-3">
                                                        <div className="d-flex align-items-center bg-white border rounded-3 px-3 shadow-sm">
                                                            <Form.Control
                                                                type="number"
                                                                step="any"
                                                                {...register("centerLocation.longitude")}
                                                                className="border-0 shadow-none px-0"
                                                            />
                                                            <span className="text-muted small ms-2 fw-bold">LNG</span>
                                                        </div>
                                                    </Form.Group>
                                                </Col>
                                                <Col xs={6} className="d-flex align-items-center gap-2">
                                                    <Form.Group className="mb-3">
                                                        <Form.Label className="text-muted fw-bold mb-2 d-block">
                                                            DISPLAY ORDER
                                                        </Form.Label>
                                                        <div className="d-flex align-items-center bg-white border rounded-3 px-3 shadow-sm" style={{ height: '45px' }}>
                                                            <Form.Control
                                                                type="number"
                                                                {...register("DisplayOrder")}
                                                                className="border-0 shadow-none px-0"
                                                                placeholder="1"
                                                            />
                                                        </div>
                                                    </Form.Group>
                                                </Col>
                                            </Row>
                                        </div>
                                    </Col>
                                </Row>
                            </div>
                            <div className='mt-4 p-4 rounded-3 shadow' style={{ backgroundColor: '#F5EFE7' }}>
                                <h6 className="fw-bold mb-4" style={{ color: "#7C572D" }}>3. Media</h6>
                                <Form.Group className="mb-3">
                                    <Form.Label className="text-muted tiny fw-bold mb-2">CITY IMAGE</Form.Label>
                                    <Form.Label htmlFor='image' className="d-flex flex-column align-items-center justify-content-center w-100 overflow-hidden border rounded-3 shadow-sm"
                                        style={{ width: '300px', height: '600px', cursor: 'pointer', backgroundColor: '#f8f9fa' }}>
                                        {posterPreview ? (
                                            <img
                                                src={posterPreview}
                                                alt="Selected Poster"
                                                style={{ width: '100%', height: '100%', objectFit: 'contain' }} />
                                        ) : (
                                            <div className="text-center p-3">
                                                <i className="bi bi-image text-secondary fs-1 d-block mb-2"></i>
                                                <span className="text-secondary small">
                                                    Click to choose a city image
                                                </span>
                                            </div>
                                        )}
                                    </Form.Label>
                                    <Form.Control id='image' type="file" accept="image/*" className="d-none" onChange={handleImage} />
                                </Form.Group>
                            </div>
                        </Form>
                    </Container>
                </div>
            </Layout>
        </>
    )
}

export default CitiesEditor